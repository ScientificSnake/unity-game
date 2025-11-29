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

            public Action<Vector2, Vector2, float, float, float> SpawnPrefab;

            public Weapon(int RPM, Action<Vector2, Vector2, float, float, float> SpawnTS, float baseMuzzleVelocity)
            {
                fireRate = RPM;
                SpawnPrefab = SpawnTS;
                BaseMuzzleVelocity = baseMuzzleVelocity;
            }
        }
        public static class WeaponryActions
        {
            public static void BasicBulletSpawnAction(Vector2 pos, Vector2 Parentveloc, float ParentRotation, float MuzzleVelo, float Accuracy)
            {
                Vector2 VelocityFromMuzzle = new Vector2(Mathf.Cos(ParentRotation * Mathf.Deg2Rad), Mathf.Sin(ParentRotation * Mathf.Deg2Rad)) * MuzzleVelo;

                Vector2 newVeloVector = VelocityFromMuzzle + Parentveloc;
                GameObject prefab = ManagerScript.Instance.BasicBulletPrefab;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
                orphan.transform.Rotate(0, 0, ParentRotation);
                BulletBehavior OrphanBulletScript = orphan.GetComponent<BulletBehavior>();
                OrphanBulletScript.velocity = newVeloVector;
            }
        }

        public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(500, WeaponryActions.BasicBulletSpawnAction, 30)
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