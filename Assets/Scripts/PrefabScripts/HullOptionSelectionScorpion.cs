using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HullOptionSelectionScorpion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string HullOptionSysName = "ScorpionHullNode";

    private GameObject RootCanvas;

    void Start()
    {
        RootCanvas = GameObject.Find("RootCanvas");
        print($"Root canvas has been set to {RootCanvas}");
    }


    public void OnClick()
    {
        print("Selecting the Scorpion hull options");

        Vector2 position = new(-329, -194);

        //ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicHullSpritePrefab, position, RootCanvas.transform); NEED REVAMP, THE HELL DOES THIS DO

        ManagerScript.CurrentLevelManagerInstance.selectedHull = HullOptionSysName;
        print(ManagerScript.CurrentLevelManagerInstance.selectedHull);
    }
}
