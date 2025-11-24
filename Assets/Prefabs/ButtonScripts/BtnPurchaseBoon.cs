using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BtnPurchaseBoon : MonoBehaviour
{
    private static string SelectedNodeSysName;
    private static int SelectedNodePrice;
    public static bool SelectedNodeDisabled;

    public Button ThisButton;
    public BoonCreditDisplayUpdater CreditDisplay;
    public GameObject ThisGameObject;
    public UpdatePurchaseBtnText DisplayText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetButtonValues(string SysName)
    {
        gameObject.SetActive(true);
        SelectedNodeSysName = SysName;

        Array dependencies = TechData.NodeDataDict[SysName].DependencyNodes;
        int price = TechData.NodeDataDict[SysName].Price;

        SelectedNodeDisabled = TechData.IsNodePurchased(SysName);
        SelectedNodePrice = TechData.NodeDataDict[SysName].Price;

        bool DependenciesMet = TechData.AreNodeDependciesMet(SysName);

        if (SelectedNodeDisabled)
        {
            DisplayText.UpdateButtonText("Already Purchased");
        }
        else if (DependenciesMet is false)
        {
            DisplayText.UpdateButtonText("Purchase Dependencies first!");
        }
        else if ((SelectedNodeDisabled is false) && (DependenciesMet))
        {
            DisplayText.UpdateButtonText($"Purchase for {SelectedNodePrice} units");
        }
    }

    // Update is called once per frame
    public void Onclick()
    {
        //print("attempting purchase");
        bool DepenciesMet = TechData.AreNodeDependciesMet(SelectedNodeSysName);
        if ((TechData.TechCredits >= SelectedNodePrice) && (SelectedNodeDisabled is false) && DepenciesMet)  // First check if you have enough money to purchase
        {
            TechData.TechCredits -= SelectedNodePrice;

            TechData.NodeDataDict[SelectedNodeSysName].IsNodePurchased = true;

            CreditDisplay.UpdateBoonCreditText();
            DisplayText.UpdateButtonText("Already Purchased");

            TechData.PurchaseNode(SelectedNodeSysName);
            SelectedNodeDisabled = true;
        }
        else if (SelectedNodeDisabled)
        {
            print("Already purchased you should display some gui element now ");
        }
        else if (TechData.TechCredits < SelectedNodePrice)
        {
            print("You do not have enough money you should probably put some gui element on the screen about now");
        }
    }
}