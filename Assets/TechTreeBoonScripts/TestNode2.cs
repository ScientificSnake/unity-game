using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestNode2 : MonoBehaviour
{
    private static string Title;
    private static string BodyText;
    private static string SysName = "SwallowHullNode";

    public BtnPurchaseBoon PurchaseButton;
    public BoonInfoDisplayTitle TargetTitleDisplay;
    public UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public static Array DependentBoonsSysNames;

    public void OnSelect()
    {
        TargetTitleDisplay.UpdateDisplay(Title);
        TargetBodyDisplay.UpdateBodytext(BodyText);

        PurchaseButton.SetButtonValues(SysName);
    }

    void Start()
    {
        DependentBoonsSysNames = TechData.HullOptionsDataDict[SysName].DependencyNodes;
        Title = TechData.HullOptionsDataDict[SysName].DisplayTitle;
        BodyText = TechData.HullOptionsDataDict[SysName].DisplayText;
    }
}

