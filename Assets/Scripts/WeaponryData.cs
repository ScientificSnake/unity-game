using UnityEngine;

namespace Sebastian{

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
        public Action SpawnPrefab;

        public Weapon(int RPM, Action<Vector2, Vector2, float> SpawnTS)
        {
            fireRate = RPM;
            SpawnPrefab = SpawnTS;
        }
    }
    public static class WeaponryActions
    {
        public void diddlingMethod(Vector2 pos, Vector2 veloc, float rotation)
        {
            veloc.magnitude += 2;
            //GameObject prefab = ManagerScript.Instance.
            GameObject orphan = ManagerScript.Instance.SpawnOrphan(prefab, pos);
            orphan.transform.Rotate(0,0,rotation);
        }
    }
    public static Dictionary<int, Weapon> = new() {
        {
            //27mm
            1, new Weapon(500, diddlingMethod())
        }
    }
}
}
/*
public float bulletScale;//Scale factor
public bool guidance;//if it has or not
public int bulletSpeed;//m/s
public int damage;
*/