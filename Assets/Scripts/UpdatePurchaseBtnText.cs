using UnityEngine;
using TMPro;
public class UpdatePurchaseBtnText : MonoBehaviour
{
    public TextMeshProUGUI ThisBtnText;

    public void UpdateButtonText(string text)
    {
        print($"Updating boon purchase button text with args {text}");
        ThisBtnText.text = text;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
