using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Sebastian
{

    /*weapons to add:
    27mm autocannon
    25mm rotary autocannon
    12.7mm machine gun
    missile
    rocket pod
    */
    public class WeaponryData
    {
        private static void ApplyIgnoredColliders(WeaponParameters param, Collider2D collider1)
        {
            foreach(Collider2D collider2 in param.IgnoredColliders)
            {
                Physics2D.IgnoreCollision(collider1, collider2);
            }
        }

        public class Weapon
        {
            public WeaponParameters BaseWeaponParams;

            public Action<WeaponParameters> SpawnPrefab;

            public Weapon(Action<WeaponParameters> SpawnTS, WeaponParameters baseWeaponParams)
            {
                BaseWeaponParams = baseWeaponParams; 
                SpawnPrefab = SpawnTS;
            }
        }

        public struct WeaponParameters
        {
            public float RPM;
            public Vector2 ParentVelo;
            public Vector2 SpawnPos;
            public float ParentZRotation;
            public float MaxDegreeError;
            public float RecoilForce;
            public float Damage;

            public List<Type> monoBehavioursAdditions;

            public List<Collider2D> IgnoredColliders;

            // ^^ Shared parameters that every weapon has

            // Parameters that are unique to that sort of weapon eg Rocket having a fuel amount
            // If they are not applicable 
            public float MuzzleVelo;
            public float AcceleratioRate;
            public float FuelSeconds;

            public float ShotDragMult;

            public GameObject Spawner;

            public int ShotCount;
            public ObjTools.InterceptData interceptData;
        }

        public static class WeaponryActions
        {
            private readonly static System.Random _random = new((int)System.DateTime.Now.Ticks);
            public static float GetRandomError(float maxError)
            {
                double random1tominus1 = (_random.NextDouble() * 2) - 1;
                return (float)random1tominus1 * maxError;
            }
            private static float GetNormalDistributedError(float maxError)
            {
                // box muller or smth. us double cuz MR whit says round to 2 decimals for z scores
                double u1 = 1.0 - _random.NextDouble();
                double u2 = 1.0 - _random.NextDouble();

                // normal dist oyeah
                double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                // this should use Mathf.clamp but its not broke so dont fix it
                standardNormal = Math.Max(-3, Math.Min(3, standardNormal));

                // Scale to max error being 3 std dev
                return (float)(standardNormal * maxError / 3.0);
            }

            private static GameObject SpawnSimpleProjectile(GameObject prefab, WeaponParameters Params, bool useError)
            {
                float trueRotation;
                if (useError)
                {
                    float ParentRotation = Params.ParentZRotation;
                    float maxDegreeError = Params.MaxDegreeError;
                    float randomOffsetFactor = GetNormalDistributedError(maxDegreeError);
                    trueRotation = ParentRotation + (float)randomOffsetFactor;
                }
                else
                {
                    trueRotation = Params.ParentZRotation;
                }

                    Vector2 VelocityFromMuzzle = new Vector2(Mathf.Cos(trueRotation * Mathf.Deg2Rad), Mathf.Sin(trueRotation * Mathf.Deg2Rad)) * Params.MuzzleVelo;

                Vector2 newVeloVector = VelocityFromMuzzle + Params.ParentVelo;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, Params.SpawnPos);
                orphan.transform.Rotate(0, 0, trueRotation);
                Rigidbody2D orphanBody = orphan.GetComponent<Rigidbody2D>();
                orphanBody.linearVelocity = newVeloVector;

                return orphan;
            }

            public static void BasicBulletSpawnAction(WeaponParameters Params)
            {
                GameObject orphan = SpawnSimpleProjectile(ManagerScript.Instance.BasicBulletPrefab, Params, true);
                BulletBehavior bulletScript = orphan.GetComponent<BulletBehavior>();

                bulletScript.Damage = Params.Damage;

                ApplyIgnoredColliders(Params, bulletScript.tcollider);

                bulletScript.tcollider.enabled = true;
                bulletScript.rb.linearDamping *= Params.ShotDragMult;

                if (Params.monoBehavioursAdditions != null)
                {
                    for (int i = 0; i < Params.monoBehavioursAdditions.Count; i++)
                    {
                        orphan.AddComponent(Params.monoBehavioursAdditions[i]);
                    }
                }
            }

            public static void BasicRocketSpawn(WeaponParameters Params)
            {
                float randomOffset = GetNormalDistributedError(Params.MaxDegreeError);
                float trueRotation = Params.ParentZRotation + randomOffset;

                GameObject prefab = ManagerScript.Instance.RocketPrefab;

                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, Params.SpawnPos);
                Rigidbody2D orphanBody = orphan.GetComponent<Rigidbody2D>();

                orphan.transform.Rotate(0, 0, trueRotation);
                orphanBody.linearVelocity += Params.ParentVelo;

                RocketBehavior rocketBehavior = orphan.GetComponent<RocketBehavior>();
                rocketBehavior.Acceleration = Params.AcceleratioRate;
                rocketBehavior.Damage = Params.Damage;
                rocketBehavior.Fuel = Params.FuelSeconds * 40; // times forty for Seconds -> ticks

                ApplyIgnoredColliders(Params, rocketBehavior.tcollider);

                rocketBehavior.tcollider.enabled = true;
            }

            public static void BasicShotGunSpawn(WeaponParameters Params)
            {
                List<Collider2D> allColliderToIgnore = new();

                for (int i = 0; i < Params.ShotCount; i++)
                {
                    GameObject bulletObj = SpawnSimpleProjectile(ManagerScript.Instance.BasicShotGunBallPrefab, Params, useError: true);

                    BulletBehavior bullet = bulletObj.GetComponent<BulletBehavior>();

                    allColliderToIgnore.Add(bullet.tcollider);

                    bullet.Damage = Params.Damage;
                }
                for(int i = 0; i < allColliderToIgnore.Count; i++)
                {
                    for (int j = i + 1; j < allColliderToIgnore.Count; j++)
                    {
                        Physics2D.IgnoreCollision(allColliderToIgnore[i], allColliderToIgnore[j]);
                    }
                    
                    ApplyIgnoredColliders(Params, allColliderToIgnore[i]);
                }
                foreach(var tcollider in allColliderToIgnore)
                {
                    tcollider.enabled = true;
                }
            }

            public static void FragGrenade(WeaponParameters Params)
            {
                List<Collider2D> allColliders = new();

                for (int i = 0; i < Params.ShotCount; i++)
                {
                    Vector2 RandDirection = (Vector2) UnityEngine.Random.onUnitSphere;
                    float RandomRotation = RandDirection.DirectionAngle();

                    GameObject fragmentPrefab = ManagerScript.Instance.BasicBulletPrefab;

                    GameObject fragment = ManagerScript.Instance.SpawnOrphan(fragmentPrefab, Params.SpawnPos);
                    Rigidbody2D fragmentRb = fragment.GetComponent<Rigidbody2D>();

                    fragmentRb.linearVelocity = (RandDirection * Params.MuzzleVelo) + Params.ParentVelo;
                    fragmentRb.SetRotation(RandomRotation);

                    BulletBehavior fragScript = fragment.GetComponent<BulletBehavior>();
                    fragScript.Damage = Params.Damage;

                    foreach (Collider2D ignoredCollider in Params.IgnoredColliders)
                    {
                        Physics2D.IgnoreCollision(ignoredCollider, fragScript.tcollider);
                    }
                    allColliders.Add(fragScript.tcollider);
                    fragScript.rb.linearDamping *= 500;
                }

                for (int i = 0; i < Params.ShotCount; i++)
                {
                    for (int j = i + 1; j < Params.ShotCount; j++)
                    {
                        Physics2D.IgnoreCollision(allColliders[i], allColliders[j]);
                    }
                }

                foreach (var collider in allColliders)
                {
                    collider.enabled = true;
                }
            }

            public static void LaunchGrenade(WeaponParameters Params)
            {
                GameObject grenadePrefab = ManagerScript.Instance.FragGrenadePrefab;
                GameObject grenade = SpawnSimpleProjectile(grenadePrefab, Params, true);

                FragGrenadeScript grenadeScript = grenade.GetComponent<FragGrenadeScript>();
                grenadeScript.FuseSeconds = Params.interceptData.Time;
                ApplyIgnoredColliders(Params, grenadeScript.tcollider);
                grenadeScript.tcollider.enabled = true;
            }
        }

        public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(WeaponryActions.BasicBulletSpawnAction,
                    new WeaponParameters {RPM = 500, MaxDegreeError = 2, MuzzleVelo = 450, RecoilForce = 0.5f, Damage = 13}
                )
            },
            {
                //25mm rotary  less accurate
                2,
                new Weapon(WeaponryActions.BasicBulletSpawnAction,
                    new WeaponParameters {RPM = 1500, MaxDegreeError = 10, MuzzleVelo = 300, RecoilForce = 0.2f, Damage = 11.5f}
                )
            },
            {
                // ze rocket
                3,
                new Weapon(WeaponryActions.BasicRocketSpawn,
                    new WeaponParameters {RPM = 30, MaxDegreeError = 4, AcceleratioRate = 500, FuelSeconds = 5, RecoilForce = 0, Damage = 500}
                )
            },
            {
                4,
                new Weapon(WeaponryActions.BasicBulletSpawnAction,
                    new WeaponParameters {RPM = 30, MaxDegreeError = 0, MuzzleVelo = 250, Damage = 200})
            },
            {
                5,
                new Weapon(WeaponryActions.BasicShotGunSpawn,
                    new WeaponParameters {RPM = 30, Damage = 75, MuzzleVelo = 150, MaxDegreeError = 15, ShotCount = 15})
            },
            {
                6,
                new Weapon(WeaponryActions.FragGrenade,
                    new WeaponParameters {Damage = 75, MuzzleVelo = 400, ShotCount = 30})
            },
            {
                7,
                new Weapon(WeaponryActions.LaunchGrenade,
                    new WeaponParameters {MuzzleVelo = 160, MaxDegreeError = 5, RPM = 60})
            }
        };
    }
}
/*
public float bulletScale;//Scale factor
public int bulletSpeed;//m/s
public int damage;
*/