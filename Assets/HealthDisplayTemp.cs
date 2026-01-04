using TMPro;
using UnityEngine;

public class HealthDisplayTemp : MonoBehaviour
{
    public static HealthScript PlayerHealthScriptRef;
    public static PlayerObjectScript Playerscript;
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
        if (Playerscript == null || PlayerRef == null || PlayerHealthScriptRef == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            Playerscript = PlayerRef.GetComponent<PlayerObjectScript>();
            PlayerHealthScriptRef = Playerscript.PlayerHealthManager;
        }
        else
        {
            tmp.text = $"<color=red> {(int)PlayerHealthScriptRef.Health} / {(int)Playerscript.BaseHealth}";
        }
    }
}
