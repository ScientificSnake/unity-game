using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

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

        public class WeaponParameters
        {
            public float RPM;
            public Vector2 ParentVelo;
            public Vector2 SpawnPos;
            public float ParentZRotation;
            public float MaxDegreeError;

            // ^^ Shared parameters that every weapon has

            // Parameters that are unique to that sort of weapon eg Rocket having a fuel amount
            // If they are not applicable 
            public float MuzzleVelo;
            public float AcceleratioRate;
            public float FuelSeconds;
        }

        public static class WeaponryActions
        {
            private static System.Random _random = new((int)System.DateTime.Now.Ticks);
            public static float GetRandomError(float maxError)
            {
                double random1tominus1 = (_random.NextDouble() * 2) - 1;
                return (float)random1tominus1 * maxError;
            }
            private static float GetNormalDistributedError(float maxError)
            {
                // Box-Muller transform for normal distribution
                double u1 = 1.0 - _random.NextDouble(); // Uniform random [0,1]
                double u2 = 1.0 - _random.NextDouble(); // Uniform random [0,1]

                // This gives you a standard normal distribution (mean=0, stddev=1)
                double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                // Clamp to ±3 standard deviations (captures 99.7% of values)
                standardNormal = Math.Max(-3, Math.Min(3, standardNormal));

                // Scale to your maxError range (treating maxError as ~3 sigma)
                return (float)(standardNormal * maxError / 3.0);
            }

            public static void BasicBulletSpawnAction(WeaponParameters Params)
            {
                // Generate within random error

                float ParentRotation = Params.ParentZRotation;
                float maxDegreeError = Params.MaxDegreeError;
                Vector2 Parentveloc = Params.ParentVelo;
                Vector2 pos = Params.SpawnPos;
                float MuzzleVelo = Params.MuzzleVelo;

                float randomOffsetFactor = GetNormalDistributedError(maxDegreeError);

                float trueRotation = ParentRotation + (float)randomOffsetFactor;

                Vector2 VelocityFromMuzzle = new Vector2(Mathf.Cos(trueRotation * Mathf.Deg2Rad), Mathf.Sin(trueRotation * Mathf.Deg2Rad)) * MuzzleVelo;

                Vector2 newVeloVector = VelocityFromMuzzle + Parentveloc;
                GameObject prefab = ManagerScript.Instance.BasicBulletPrefab;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
                orphan.transform.Rotate(0, 0, trueRotation);
                Rigidbody2D orphanBody = orphan.GetComponent<Rigidbody2D>();
                orphanBody.linearVelocity = newVeloVector;
            }

            public static void BasicRocketSpawn(WeaponParameters Params)
            {
                float randomOffset = GetNormalDistributedError(Params.MaxDegreeError);
                float trueRotation = Params.ParentZRotation + randomOffset;

                GameObject prefab = ManagerScript.Instance.RocketPrefab;

                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, Params.SpawnPos);
                Rigidbody2D orphanBody = orphan.GetComponent<Rigidbody2D>();
                orphanBody.linearVelocity = Params.ParentVelo;
                orphan.transform.Rotate(0, 0, trueRotation);

                RocketBehavior rocketBehavior = orphan.GetComponent<RocketBehavior>();
                rocketBehavior.Acceleration = Params.AcceleratioRate;
                rocketBehavior.Fuel = Params.FuelSeconds * 40; // times forty for Seconds -> ticks

            }
        }

        public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(WeaponryActions.BasicBulletSpawnAction,
                    new WeaponParameters {RPM = 500, MaxDegreeError = 2, MuzzleVelo = 200}
                )
            },
            {
                //25mm rotary  less accurate
                2,
                new Weapon(WeaponryActions.BasicBulletSpawnAction,
                    new WeaponParameters {RPM = 1500, MaxDegreeError = 10, MuzzleVelo = 200}
                )
            },
            {
                // ze rocket
                3,
                new Weapon(WeaponryActions.BasicRocketSpawn,
                    new WeaponParameters {RPM = 30, MaxDegreeError = 4, AcceleratioRate = 500, FuelSeconds = 5}
                )
            }
        };
    }
}
/*
public float bulletScale;//Scale factor
public bool guidance;//if it has or not
public int bulletSpeed;//m/s
public int damage;
*/