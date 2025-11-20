using UnityEngine;
using TMPro;

public class BoonCreditDisplayUpdater : MonoBehaviour
{
    public TextMeshProUGUI BoonCreditText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateBoonCreditText()
    {
        this.BoonCreditText.text = (ManagerScript.BoonCredits).ToString();
    }


    void Start()
    {
        this.UpdateBoonCreditText();
    }

    // Update is called once per frame
    void Update()
    {
    }


}
