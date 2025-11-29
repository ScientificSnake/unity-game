using System;
using System.Collections.Generic;
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
            public static void BasicBulletSpawnAction(Vector2 pos, Vector2 Parentveloc, float ParentRotation, float MuzzleVelo, float maxDegreeError)
            {
                // Generate within random error
                System.Random NewRandom = new((int)System.DateTime.Now.Ticks);  // maybe should use normal distribution so less offset values are more common

                double randomOffsetFactor = NewRandom.NextDouble();
                randomOffsetFactor *= 2;
                randomOffsetFactor -= 1;
                randomOffsetFactor *= maxDegreeError;

                float trueRotation = ParentRotation + (float) randomOffsetFactor;   

                Vector2 VelocityFromMuzzle = new Vector2(Mathf.Cos(trueRotation * Mathf.Deg2Rad), Mathf.Sin(trueRotation * Mathf.Deg2Rad)) * MuzzleVelo;

                Vector2 newVeloVector = VelocityFromMuzzle + Parentveloc;
                GameObject prefab = ManagerScript.Instance.BasicBulletPrefab;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
                orphan.transform.Rotate(0, 0, trueRotation);
                BulletBehavior OrphanBulletScript = orphan.GetComponent<BulletBehavior>();
                OrphanBulletScript.velocity = newVeloVector;
            }
        }

        public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(500, WeaponryActions.BasicBulletSpawnAction, 25, 2)
            },
            {
                //25mm rotary
                2,
                new Weapon(1500, WeaponryActions.BasicBulletSpawnAction, 20, 10)
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