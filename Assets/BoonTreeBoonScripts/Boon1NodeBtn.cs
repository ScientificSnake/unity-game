using UnityEngine;

public class Boon1NodeBtn : MonoBehaviour
{
    private static string Title = "Boon 1 placeholder";
    private static string BodyText = "Insane multi line description about the cost and features";
    private static string SysName = "TestBoon1";
    private static int Price = 30;

    public BtnPurchaseBoon PurchaseButton;
    public BoonInfoDisplayTitle TargetTitleDisplay;
    public UpdateBoonBodyTextDisplay TargetBodyDisplay;

    public void OnSelect()
    {
        TargetTitleDisplay.UpdateDisplay(Title);
        TargetBodyDisplay.UpdateBodytext(BodyText);

        PurchaseButton.SetButtonValues(SysName, Price);
    }

    void Start()
    {
        // Boon1Purchased =
    }
}
