using UnityEngine;

public class BtnSettingsDropDown : MonoBehaviour
{
    public GameObject saveInterfacePanel;

    public void OnClick()
    {
        saveInterfacePanel.SetActive(true);
    }

    public void Awake()
    {
        saveInterfacePanel.SetActive(false);
    }
}
