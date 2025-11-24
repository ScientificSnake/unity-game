using System;
using UnityEngine;

public class ScorpionBtnScript : MonoBehaviour
{
    private static string Title;
    private static string BodyText;
    private static string SysName = "ScorpionHullNode";

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
        DependentBoonsSysNames = TechData.NodeDataDict[SysName].DependencyNodes;
        Title = TechData.NodeDataDict[SysName].DisplayTitle;
        BodyText = TechData.NodeDataDict[SysName].DisplayText;
    }
}
