using TMPro;
using UnityEngine;

public class HealthDisplayTemp : MonoBehaviour
{
    public static PlayerObjectScript PlayerScriptRef;
    public static GameObject PlayerRef;

    public static TextMeshProUGUI tmp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScriptRef == null || PlayerRef == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            PlayerScriptRef = PlayerRef.GetComponent<PlayerObjectScript>();
        }
        else
        {
            tmp.text = $"<color=red> {(int)PlayerScriptRef.Health} / {(int)PlayerScriptRef.BaseHealth}";
        }
    }
}
