using UnityEngine;

public class ProjInterceptCalc
{
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

        if (Mathf.Abs(a) > 0.0001f) {
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
