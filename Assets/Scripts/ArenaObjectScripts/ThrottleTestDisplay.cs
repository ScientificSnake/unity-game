using TMPro;
using UnityEngine;

public class ThrottleTestDisplay : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public PlayerObjectScript PlayerRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject PlayerGO = GameObject.FindWithTag("Player");
        PlayerRef = PlayerGO.GetComponent<PlayerObjectScript>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = PlayerRef.throttle.ToString();
    }
}
