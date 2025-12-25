using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoundsEnforcer : MonoBehaviour
{
    public static List<IBoundsCheckable> TrackedObjects = new List<IBoundsCheckable>();

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
        foreach (IBoundsCheckable Obj in TrackedObjects.ToList())
        {
            if (IsInBounds(Obj) == false)
                Obj.OnOutOfBounds();
        }
    }
    
    public bool IsInBounds(IBoundsCheckable Obj)
    {
        float x = Obj.Rigidbody2.gameObject.transform.position.x;
        float y = Obj.Rigidbody2.gameObject.transform.position.y;

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
