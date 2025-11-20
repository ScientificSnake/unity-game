using UnityEngine;
using TMPro;


public class UpdateBoonBodyTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI BodyText;
    public void UpdateBodytext(string BodyTextInput)
    {
        this.BodyText.text = BodyTextInput;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BodyText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
