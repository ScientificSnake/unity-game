using System;
using UnityEngine;

public class BaseHullBtnScript : MonoBehaviour
{
    private string Title;
    private string BodyText;
    public string SysName;

    public static BtnPurchaseBoon PurchaseButton;
    public static BoonInfoDisplayTitle TargetTitleDisplay;
    public static UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public Array DependentBoonsSysNames;

    public void Awake()
    {
        if (PurchaseButton == null)
            PurchaseButton = GameObject.Find("BoonPurchaseButton").GetComponent<BtnPurchaseBoon>();
        if (TargetBodyDisplay == null)
            TargetTitleDisplay = GameObject.Find("SelectedBoonTitle").GetComponent<BoonInfoDisplayTitle>();
        if (TargetBodyDisplay == null)
            TargetBodyDisplay = GameObject.Find("SelectedBoonBodyText").GetComponent<UpdateBoonBodyTextDisplay>();
    }


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
