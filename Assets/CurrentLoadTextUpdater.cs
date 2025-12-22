using TMPro;
using UnityEngine;

public class CurrentLoadTextUpdater : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void UpdateText()
    {
        tmp.text = $"Current load: {ManagerScript.Instance.CurrentLoad}";
    }

    public void Start()
    {
        tmp.text = $"Current load: {ManagerScript.Instance.CurrentLoad}";
    }
}
