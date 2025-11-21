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
    public static List<string> SelectedNodeDependencies;

    public Button ThisButton;
    public BoonCreditDisplayUpdater CreditDisplay;
    public GameObject ThisGameObject;
    public UpdatePurchaseBtnText DisplayText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetButtonValues(string SysName, int Price, List<string> Dependencies)
    {
        gameObject.SetActive(true);

        SelectedNodeDependencies = Dependencies;
        SelectedNodeSysName = SysName;
        SelectedNodePrice = Price;
        SelectedNodeDisabled = ManagerScript.Instance.IsNodePurchased(SysName);

        bool DependenciesMet = ManagerScript.Instance.AreNodeDependciesMet(Dependencies);

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
        print("attempting purchase");
        bool DepenciesMet = ManagerScript.Instance.AreNodeDependciesMet(SelectedNodeDependencies);

        print($"Depencies met :{DepenciesMet}");
        print($"Selcted node disabled : {SelectedNodeDisabled.ToString()}");
        print($"Enough money : {(ManagerScript.TechCredits >= SelectedNodePrice).ToString()}");
        if ((ManagerScript.TechCredits >= SelectedNodePrice) && (SelectedNodeDisabled is false) && DepenciesMet)  // First check if you have enough money to purchase
        {
            ManagerScript.TechCredits -= SelectedNodePrice;

            ManagerScript.UnlockedBoons.Add(SelectedNodeSysName);

            CreditDisplay.UpdateBoonCreditText();
            DisplayText.UpdateButtonText("Already Purchased");

            ManagerScript.Instance.PurchaseNode(SelectedNodeSysName);
            SelectedNodeDisabled = true;
        }
        else if (SelectedNodeDisabled)
        {
            print("Already purchased you should display some gui element now ");
        }
        else if (ManagerScript.TechCredits < SelectedNodePrice)
        {
            print("You do not have enough money you should probably put some gui element on the screen about now");
        }
    }
}