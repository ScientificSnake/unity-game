using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ObjTools
{
    public static bool LineOfSight(GameObject StartObject, Transform targetTransform, float MaxDistance)
    {
        Vector2 directionToTarget = targetTransform.position - StartObject.transform.position;

        StartObject.GetComponent<Collider2D>().enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(StartObject.transform.position, directionToTarget, MaxDistance);
        StartObject.GetComponent<Collider2D>().enabled = true;

        // Check if raycast hit anything and if it's the player
        if (hit.collider != null && hit.transform == targetTransform)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ScaleThrusterRefs(List<GameObject> thrusterRefs, List<Vector2> thrusterBaseScales, float throttle)
    {
        for (int i = 0; i < thrusterRefs.Count; i++)
        {
            Vector2 targetScale = thrusterBaseScales[i] * (throttle / 100);
            thrusterRefs[i].gameObject.transform.localScale = targetScale;
        }
    }

    public class InterceptData
    {
        public bool Possible;
        public Vector2 AimDir;
        public float AimDeg;
    }
    public static InterceptData TryGetInterceptAngle(
    Vector2 shooterPos,
    Vector2 shooterVel,
    Vector2 targetPos,
    Vector2 targetVel,
    float bulletSpeed)
    {
        InterceptData returnData = new();
        Vector2 r = targetPos - shooterPos;
        Vector2 v = targetVel - shooterVel;

        float a = Vector2.Dot(v, v) - bulletSpeed * bulletSpeed;
        float b = 2f * Vector2.Dot(r, v);
        float c = Vector2.Dot(r, r);

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f)
        {
            returnData.Possible = false;
            return returnData;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);

        float t1;
        float t2;

        if (Mathf.Abs(a) > 0.0001f)
        {
            t1 = (-b - sqrtDisc) / (2f * a);
            t2 = (-b + sqrtDisc) / (2f * a);
        }
        else
        {
            returnData.Possible = false;
            return returnData;
        }
        float t = Mathf.Min(t1, t2);
        if (t <= 0f)
            t = Mathf.Max(t1, t2);
        if (t <= 0f)
        {
            returnData.Possible = false;
            return returnData;
        }
        Vector2 aimDir = (r + v * t).normalized;
        float angleDeg = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        returnData.Possible = true;
        returnData.AimDir = aimDir;
        returnData.AimDeg = angleDeg;

        return returnData;
    }

}
public class FixedSizeQueue<T>
{
    private Queue<T> queue = new Queue<T>();
    private int maxSize;

    public FixedSizeQueue(int maxSize)
    {
        this.maxSize = maxSize;
    }

    public void Enqueue(T item)
    {
        if (!queue.Contains(item))
        {
            if (queue.Count >= maxSize)
            {
                queue.Dequeue(); // Remove oldest item
            }
            queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        return queue.Dequeue();
    }

    public int Count => queue.Count;

    public void Clear()
    {
        queue.Clear();
    }

    public List<T> ToList()
    {
        return queue.ToList();
    }
}