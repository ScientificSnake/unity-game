using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ObjTools
{
    public static void ApplyThrottle(float throttle, ref float fuel, float maxAccleration, Rigidbody2D rb, Transform transform, float fuelUsage)
    {
        if (throttle > 0 && fuel > 0)
        {
            float trueThrottleProportion = (throttle / 100);

            float instantaneousAcceleration = trueThrottleProportion * maxAccleration;

            float heading_rad = Mathf.Deg2Rad * (transform.eulerAngles.z);

            Vector2 instantaneousAccelerationVector = new Vector2((Mathf.Cos(heading_rad) * instantaneousAcceleration), (Mathf.Sin(heading_rad) * instantaneousAcceleration));

            rb.AddForce(instantaneousAccelerationVector);

            float fuelUse = trueThrottleProportion * fuelUsage;
            fuel -= fuelUse;
        }
    }


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

#region diddy ah claude (not implemented) tried to unicode (bom)
public static class AcceleratingIntercept
{
    public class InterceptResult
    {
        public bool Possible;
        public Vector2 AimDir;
        public float AimDeg;
        public float TimeToIntercept;
        public Vector2 InterceptPoint;
    }

    /// <summary>
    /// Calculates intercept for an object that accelerates in the direction it's pointing.
    /// This is an iterative approximation since there's no closed-form solution.
    /// </summary>
    /// <param name="shooterPos">Current position of pursuing object</param>
    /// <param name="shooterVel">Current velocity of pursuing object</param>
    /// <param name="shooterAccel">Maximum acceleration magnitude of pursuing object</param>
    /// <param name="targetPos">Current position of target</param>
    /// <param name="targetVel">Current velocity of target</param>
    /// <param name="maxIterations">Maximum iterations for convergence (default 10)</param>
    /// <param name="tolerance">Convergence tolerance in units (default 1.0)</param>
    /// <returns>InterceptResult with aim direction and whether intercept is possible</returns>
    public static InterceptResult CalculateAcceleratingIntercept(
        Vector2 shooterPos,
        Vector2 shooterVel,
        float shooterAccel,
        Vector2 targetPos,
        Vector2 targetVel,
        int maxIterations = 10,
        float tolerance = 1f)
    {
        InterceptResult result = new InterceptResult();

        // Initial guess: use time to reach target at current speed + acceleration boost
        Vector2 toTarget = targetPos - shooterPos;
        float distance = toTarget.magnitude;

        float currentSpeed = shooterVel.magnitude;
        float a = 0.5f * shooterAccel;
        float b = currentSpeed;
        float c = -distance;

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            result.Possible = false;
            return result;
        }

        float timeEstimate = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        if (timeEstimate <= 0)
        {
            result.Possible = false;
            return result;
        }

        // Iteratively refine the intercept solution
        Vector2 interceptPoint = targetPos;
        float interceptTime = timeEstimate;

        for (int i = 0; i < maxIterations; i++)
        {
            // Predict where target will be at this time
            Vector2 predictedTargetPos = targetPos + targetVel * interceptTime;

            // Calculate where we need to aim
            Vector2 toIntercept = predictedTargetPos - shooterPos;
            float distanceToIntercept = toIntercept.magnitude;

            if (distanceToIntercept < 0.01f)
            {
                // Already at target
                result.Possible = true;
                result.InterceptPoint = predictedTargetPos;
                result.TimeToIntercept = 0;
                result.AimDir = toTarget.normalized;
                result.AimDeg = Mathf.Atan2(result.AimDir.y, result.AimDir.x) * Mathf.Rad2Deg;
                return result;
            }

            Vector2 aimDirection = toIntercept.normalized;

            // Calculate time to reach intercept point with constant acceleration in aim direction
            float velocityInAimDir = Vector2.Dot(shooterVel, aimDirection);

            float a_coef = 0.5f * shooterAccel;
            float b_coef = velocityInAimDir;
            float c_coef = -distanceToIntercept;

            float disc = b_coef * b_coef - 4 * a_coef * c_coef;
            if (disc < 0)
            {
                result.Possible = false;
                return result;
            }

            float newTime = (-b_coef + Mathf.Sqrt(disc)) / (2 * a_coef);
            if (newTime <= 0)
            {
                newTime = (-b_coef - Mathf.Sqrt(disc)) / (2 * a_coef);
            }

            if (newTime <= 0)
            {
                result.Possible = false;
                return result;
            }

            // Check convergence
            if (Mathf.Abs(newTime - interceptTime) < 0.01f ||
                Vector2.Distance(predictedTargetPos, interceptPoint) < tolerance)
            {
                // Converged!
                result.Possible = true;
                result.InterceptPoint = predictedTargetPos;
                result.TimeToIntercept = newTime;
                result.AimDir = aimDirection;
                result.AimDeg = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                return result;
            }

            // Update for next iteration
            interceptTime = newTime;
            interceptPoint = predictedTargetPos;
        }

        // Didn't converge, but return best guess
        result.Possible = true;
        result.InterceptPoint = interceptPoint;
        result.TimeToIntercept = interceptTime;
        Vector2 finalAimDir = (interceptPoint - shooterPos).normalized;
        result.AimDir = finalAimDir;
        result.AimDeg = Mathf.Atan2(finalAimDir.y, finalAimDir.x) * Mathf.Rad2Deg;

        return result;
    }

    /// <summary>
    /// Simpler proportional navigation approach - often better for games
    /// Aims at a point ahead of the target based on relative velocities and acceleration capability
    /// </summary>
    public static InterceptResult ProportionalNavigation(
        Vector2 shooterPos,
        Vector2 shooterVel,
        float shooterAccel,
        Vector2 targetPos,
        Vector2 targetVel,
        float leadTimeFactor = 1.5f)
    {
        InterceptResult result = new InterceptResult();

        Vector2 relativeVel = targetVel - shooterVel;
        Vector2 toTarget = targetPos - shooterPos;
        float distance = toTarget.magnitude;

        if (distance < 0.01f)
        {
            result.Possible = false;
            return result;
        }

        // Estimate time based on current closing speed and acceleration
        float closingSpeed = -Vector2.Dot(relativeVel, toTarget.normalized);

        // Time estimate considering acceleration
        float effectiveSpeed = Mathf.Max(closingSpeed, 0) + shooterAccel * leadTimeFactor;
        float leadTime = distance / Mathf.Max(effectiveSpeed, 1f);

        // Predict target position
        Vector2 predictedPos = targetPos + targetVel * leadTime;
        Vector2 aimDir = (predictedPos - shooterPos).normalized;

        result.Possible = true;
        result.InterceptPoint = predictedPos;
        result.TimeToIntercept = leadTime;
        result.AimDir = aimDir;
        result.AimDeg = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        return result;
    }

    /// <summary>
    /// Pursuit curve approach - continuously aim at current target position
    /// This naturally creates an intercept curve for accelerating objects
    /// Best for kamikaze-style enemies
    /// </summary>
    public static InterceptResult PursuitCurve(
        Vector2 shooterPos,
        Vector2 targetPos)
    {
        InterceptResult result = new();

        Vector2 toTarget = targetPos - shooterPos;
        float distance = toTarget.magnitude;

        if (distance < 0.01f)
        {
            result.Possible = false;
            return result;
        }

        Vector2 aimDir = toTarget.normalized;

        result.Possible = true;
        result.InterceptPoint = targetPos;
        result.TimeToIntercept = -1; // Unknown for pursuit curve
        result.AimDir = aimDir;
        result.AimDeg = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        return result;
    }
}

// Example usage in your KamikazeEnemyAI:
/*
protected void FixedUpdate()
{
    UpdateState();
    if (State == "LockedPlayer")
    {
        // Method 1: Full iterative solution (most accurate but more expensive)
        AcceleratingIntercept.InterceptResult intercept = 
            AcceleratingIntercept.CalculateAcceleratingIntercept(
                transform.position,
                rb.linearVelocity,
                MaxAccel,
                PlayerRef.transform.position,
                PlayerScriptRef.rb.linearVelocity
            );
        
        // Method 2: Proportional navigation (good balance)
        // AcceleratingIntercept.InterceptResult intercept = 
        //     AcceleratingIntercept.ProportionalNavigation(
        //         transform.position,
        //         rb.linearVelocity,
        //         MaxAccel,
        //         PlayerRef.transform.position,
        //         PlayerScriptRef.rb.linearVelocity
        //     );
        
        // Method 3: Simple pursuit (simplest, often best for kamikaze)
        // AcceleratingIntercept.InterceptResult intercept = 
        //     AcceleratingIntercept.PursuitCurve(
        //         transform.position,
        //         PlayerRef.transform.position
        //     );
        
        if (intercept.Possible)
        {
            RotateTowardsTargetAngle(intercept.AimDeg);
            
            // Always thrust when locked on
            Throttle = 100;
        }
        else
        {
            Throttle = 0;
        }
    }
    else
    {
        Throttle = 0;
    }
    
    ObjTools.ApplyThrottle(Throttle, ref Fuel, MaxAccel, rb, transform, FuelUsage);
}
*/

#endregion
