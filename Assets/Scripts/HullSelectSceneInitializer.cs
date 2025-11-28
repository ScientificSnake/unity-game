using UnityEngine;

public class HullSelectSceneInitializer : MonoBehaviour
{
    public GameObject RootCanvas;

    void Start()
    {
        print("Starting arena scene initializer");


        // Access the data payload we saved in ManagerScript before the scene change
        if (ManagerScript.CurrentLevelManagerInstance != null)
        {
            ManagerScript.CurrentLevelManagerInstance.DisplayHullSelectionMenu(RootCanvas.transform);
        }
        else
        {
            Debug.LogError("Arena scene loaded without a valid LevelManager instance!");
        }
    }
}