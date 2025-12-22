using System;
using UnityEngine;

public class BaseHullBtnScript : MonoBehaviour
{
    private string Title;
    private string BodyText;
    public string SysName;

    public BtnPurchaseBoon PurchaseButton;
    public BoonInfoDisplayTitle TargetTitleDisplay;
    public UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public Array DependentBoonsSysNames;

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
