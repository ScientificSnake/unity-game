using System.Collections.Generic;
using UnityEngine;

public class TestNode2 : MonoBehaviour
{
    private static string Title = "Node 2 placeholder";
    private static string BodyText = "Major Boon: ";
    private static string SysName = "TestNode2";
    private static int Price = 67;

    public BtnPurchaseBoon PurchaseButton;
    public BoonInfoDisplayTitle TargetTitleDisplay;
    public UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public static List<string> DependentBoonsSysNames = new List<string>
    {
        "TestBoon1"
    };


    public void OnSelect()
    {
        TargetTitleDisplay.UpdateDisplay(Title);
        TargetBodyDisplay.UpdateBodytext(BodyText);

        PurchaseButton.SetButtonValues(SysName, Price, DependentBoonsSysNames);
    }

    void Start()
    {
        // Boon1Purchased =
    }
}
