using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BtnPurchaseBoon : MonoBehaviour
{
    private static string SelectedBoonSysName;
    private static int SelectedBoonPrice;
    public static bool SelectedBoonDisabled;

    public Button ThisButton;
    public BoonCreditDisplayUpdater CreditDisplay;
    public GameObject ThisGameObject;
    public UpdatePurchaseBtnText DisplayText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetButtonValues(string SysName, int Price)
    {
        gameObject.SetActive(true);
        SelectedBoonSysName = SysName;
        SelectedBoonPrice = Price;
        SelectedBoonDisabled = ManagerScript.IsBoonPurchased[SysName];

        if (SelectedBoonDisabled)
        {
            ThisButton.interactable = false;

            DisplayText.UpdateButtonText("Already Purchased");
        }
        else
        {
            ThisButton.interactable = true;
            DisplayText.UpdateButtonText($"Purchase for {SelectedBoonPrice} units");
        }
    }

    // Update is called once per frame
    public void Onclick()
    {
        // First check if you have enough money to purchase
        if ((ManagerScript.BoonCredits >= SelectedBoonPrice) && SelectedBoonDisabled is false)
        {
            ManagerScript.BoonCredits -= SelectedBoonPrice;

            ManagerScript.UnlockedBoons.Add(SelectedBoonSysName);

            CreditDisplay.UpdateBoonCreditText();
            DisplayText.UpdateButtonText("Already Purchased");

            ManagerScript.IsBoonPurchased[SelectedBoonSysName] = true;
            SelectedBoonDisabled = true;
        }
        else if (SelectedBoonDisabled)
        {
            print("Already purchased you should display some gui element now ");
        }
        else if (ManagerScript.BoonCredits < SelectedBoonPrice)
        {
            print("You do not have enough money you should probably put some gui element on the screen about now");
        }
    }
}