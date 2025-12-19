using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BasicHullOptionSelectPrefab : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string HullOptionSysName = "BasicHullNode";

    private GameObject RootCanvas;

    void Start()
    {
        RootCanvas = GameObject.Find("RootCanvas");
        print($"Root canvas has been set to {RootCanvas}");
    }


    public void OnClick()
    {
        print("Selecting the Basic hull options");

        Vector2 position = new(-329, -194);

        ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicHullSpritePrefab, position, RootCanvas.transform);

        ManagerScript.CurrentLevelManagerInstance.selectedHull = HullOptionSysName;
        print(ManagerScript.CurrentLevelManagerInstance.selectedHull);
    }
}
