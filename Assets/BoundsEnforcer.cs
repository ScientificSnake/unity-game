using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BoundsEnforcer : MonoBehaviour
{
    private static List<IBoundsCheckable> TrackedObjects = new List<IBoundsCheckable>();

    public static Vector2 MinBounds;
    public static Vector2 MaxBounds;

    public static void Register(IBoundsCheckable obj)
    {
        TrackedObjects.Add(obj);
    }

    public void UpdateBounds()
    {
        MinBounds = ManagerScript.CurrentLevelManagerInstance.RootLevelData.MinXY;
        MaxBounds = ManagerScript.CurrentLevelManagerInstance.RootLevelData.MaxXY;
    }

    private void Awake()
    {
        UpdateBounds();

        print($"<color=purple> min bounds are {MinBounds}, max bounds are {MaxBounds}");
    }

    public static void DeRegister(IBoundsCheckable obj) {
        TrackedObjects.Remove(obj);
    }

    private void FixedUpdate()
    {
        // Use a standard for-loop to avoid the .ToList() "Ghost Reference" error
        for (int i = TrackedObjects.Count - 1; i >= 0; i--)
        {
            IBoundsCheckable Obj = TrackedObjects[i];

            // Unity-specific null check (catches destroyed objects)
            if (Obj == null || Obj.Rigidbody2 == null)
            {
                TrackedObjects.RemoveAt(i);
                continue;
            }

            if (IsInBounds(Obj) == false)
            {
                Obj.OnOutOfBounds();
            }
        }
    }

    public bool IsInBounds(IBoundsCheckable Obj)
    {
        // Access position directly from Rigidbody2D (more reliable than transform.position)
        float x = Obj.Rigidbody2.position.x;
        float y = Obj.Rigidbody2.position.y;

        // Safety: If bounds are exactly 0,0 (not yet loaded), don't kill the object
        if (MinBounds == Vector2.zero && MaxBounds == Vector2.zero) return true;

        if (y < MinBounds.y || y > MaxBounds.y)
        {
            return false;
        }
        if (x < MinBounds.x || x > MaxBounds.x)
        {
            return false;
        }
        return true;
    }
}

public interface IBoundsCheckable
{
    public Rigidbody2D Rigidbody2 { get; }
    public void OnOutOfBounds();
}
