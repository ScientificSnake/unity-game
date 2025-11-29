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
            public int fireRate;//RPM
            public float BaseMuzzleVelocity;
            public float BaseMaxDegreeError;

            public Action<Vector2, Vector2, float, float, float> SpawnPrefab;

            public Weapon(int RPM, Action<Vector2, Vector2, float, float, float> SpawnTS, float baseMuzzleVelocity, float baseMaxDegreeError)
            {
                fireRate = RPM;
                SpawnPrefab = SpawnTS;
                BaseMuzzleVelocity = baseMuzzleVelocity;
                BaseMaxDegreeError = baseMaxDegreeError;
            }
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

            public static void BasicBulletSpawnAction(Vector2 pos, Vector2 Parentveloc, float ParentRotation, float MuzzleVelo, float maxDegreeError)
            {
                // Generate within random error
                System.Random NewRandom = new((int)DateTime.Now.Ticks);  // maybe should use normal distribution so less offset values are more common

                float randomOffsetFactor = GetNormalDistributedError(maxDegreeError);

                float trueRotation = ParentRotation + (float)randomOffsetFactor;

                Vector2 VelocityFromMuzzle = new Vector2(Mathf.Cos(trueRotation * Mathf.Deg2Rad), Mathf.Sin(trueRotation * Mathf.Deg2Rad)) * MuzzleVelo;

                Vector2 newVeloVector = VelocityFromMuzzle + Parentveloc;
                GameObject prefab = ManagerScript.Instance.BasicBulletPrefab;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
                orphan.transform.Rotate(0, 0, trueRotation);
                BulletBehavior OrphanBulletScript = orphan.GetComponent<BulletBehavior>();
                OrphanBulletScript.velocity = newVeloVector;
            }

            public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(650, WeaponryActions.BasicBulletSpawnAction, 25, 2)
            },
            {
                //25mm rotary
                2,
                new Weapon(1500, WeaponryActions.BasicBulletSpawnAction, 20, 10)

            }
        };
        }
    }
}