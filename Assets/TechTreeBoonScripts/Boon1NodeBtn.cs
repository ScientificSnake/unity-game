using System;
using UnityEngine;

public class Boon1NodeBtn : MonoBehaviour
{
    private static string Title;
    private static string BodyText;
    private readonly static string SysName = "BasicHullNode";

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
