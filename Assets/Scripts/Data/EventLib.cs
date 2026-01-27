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

    private static readonly float KamikazeSpawnNeededSpace = 20f;

    private static void SpawnPrefabAtRad(float radius, float neededSpace, GameObject prefab)
    {
        // ok so the idea is randomly pick a position on the circle and do a physics overlap circle
        for (int i = 0; i < MaxSpawnTrys; i++)
        {
            Vector2 direction = (Vector2)UnityEngine.Random.onUnitSphere;
            Vector2 pos = direction * radius;

            Collider2D HitColldier = Physics2D.OverlapCircle(pos, neededSpace   );

            if (HitColldier != null)
            {
                continue; // try spawning again
            }

            ManagerScript.Instance.SpawnOrphan(prefab, pos);
        }
    }
}