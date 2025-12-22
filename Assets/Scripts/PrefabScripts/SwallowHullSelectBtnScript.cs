using UnityEngine;

public class SwallowHullSelectBtnScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string HullOptionSysName = "SwallowHullNode";

    private GameObject RootCanvas;

    void Start()
    {
        RootCanvas = GameObject.Find("RootCanvas");
        print($"Root canvas has been set to {RootCanvas}");
    }


    public void OnClick()
    {
        // print("Selecting the Basic hull options");

        Vector2 position = new(-329, -194);

        //ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicHullSpritePrefab, position, RootCanvas.transform);

        ManagerScript.CurrentLevelManagerInstance.selectedHull = HullOptionSysName;
        print($"Selected hull is now { ManagerScript.CurrentLevelManagerInstance.selectedHull}");
    }
}
