using UnityEngine;

public class TrohpyHulloptionSelectnBtn : MonoBehaviour
{
    private string HullOptionSysName = "TrophyHullNode";

    private GameObject RootCanvas;

    void Start()
    {
        RootCanvas = GameObject.Find("RootCanvas");
        print($"Root canvas has been set to {RootCanvas}");
    }


    public void OnClick()
    {
        Vector2 position = new(-329, -194);

        //ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicHullSpritePrefab, position, RootCanvas.transform);

        ManagerScript.CurrentLevelManagerInstance.selectedHull = HullOptionSysName;
        print($"Selected hull is now {ManagerScript.CurrentLevelManagerInstance.selectedHull}");
    }
}
