using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boon1NodeBtn : MonoBehaviour
{
    private static string Title = "Boon 1 placeholder";
    private static string BodyText = "Insane multi line description about the cost and features";
    private static string SysName = "TestBoon1";
    private static int Price = 30;

    public BtnPurchaseBoon PurchaseButton;
    public BoonInfoDisplayTitle TargetTitleDisplay;
    public UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public static List<string> DependentBoonsSysNames = new List<string>
    {
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
