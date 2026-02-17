using System;
using UnityEngine;

public static class EventLib
{
    public static int MaxSpawnTrys;
    public static void SpawnTutorialDummy()
    {
        Debug.Log("Spawning test dummy");
    }

    public static void TestEvent()
    {
        Debug.Log("test event. maybe a solar flare that damages");
    }

    public static void SpawnKamikazeAtRad75()
    {
        Debug.Log($"<color=5e9bd1> ATTEMPTING TO SPAWN KAMIKAZE AT RAD 75");
        SpawnPrefabAtRad(75, KamikazeSpawnNeededSpace, ManagerScript.Instance.KamikazePrefab);
    }

    private static readonly float KamikazeSpawnNeededSpace = 20f;

    private static void SpawnPrefabAtRad(float radius, float neededSpace, GameObject prefab)
    {
        // ok so the idea is randomly pick a position on the circle and do a physics overlap circle
        for (int i = 0; i < MaxSpawnTrys; i++)
        {
            Vector2 direction = (Vector2)UnityEngine.Random.onUnitSphere;
            Vector2 pos = direction * radius;

            Collider2D HitColldier = Physics2D.OverlapCircle(pos, neededSpace);

            if (HitColldier != null)
            {
                continue; // try spawning again
            }

            ManagerScript.Instance.SpawnOrphan(prefab, pos);
        }
    }
}