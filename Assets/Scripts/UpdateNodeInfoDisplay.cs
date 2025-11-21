using UnityEngine;
using TMPro;

public class BoonInfoDisplayTitle : MonoBehaviour
{
    public TextMeshProUGUI BoonInfoTitle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateDisplay(string title)
    {
        this.BoonInfoTitle.text = title;
    }

    private void Start()
    {
        UpdateDisplay("");
        // Start blank
    }
}
