using System;
using System.Collections.Generic;
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
        public class Weapon
        {
            public int fireRate;//RPM
            public float MuzzleVelocity;

            public Action<Vector2, Vector2, float, float, float> SpawnPrefab;

            public Weapon(int RPM, Action<Vector2, Vector2, float, float, float> SpawnTS)
            {
                fireRate = RPM;
                SpawnPrefab = SpawnTS;
            }
        }
        public static class WeaponryActions
        {
            public static void diddlingMethod(Vector2 pos, Vector2 Parentveloc, float ParentRotation, float MuzzleVelo, float Accuracy)
            {
                Vector2 newVeloVector = Parentveloc.normalized * (Parentveloc.magnitude + 2);
                GameObject prefab = ManagerScript.Instance.BasicBulletPrefab;
                GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
                orphan.transform.Rotate(0, 0, ParentRotation);
            }
        }

        public static Dictionary<int, Weapon> WeaponDict = new()
        {
            {
                //27mm
                1,
                new Weapon(500, WeaponryActions.diddlingMethod)
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