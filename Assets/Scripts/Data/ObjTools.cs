using UnityEngine;

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
}
