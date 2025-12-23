using UnityEngine;

public class BtnSettingsDropDown : MonoBehaviour
{
    public GameObject saveInterfacePanel;
    public RecentSaveDropDown recentSaveDropDown;

    public void OnClick()
    {
        saveInterfacePanel.SetActive(true);
        recentSaveDropDown.refreshOptions();
    }

    public void Awake()
    {
        saveInterfacePanel.SetActive(false);
    }
}
