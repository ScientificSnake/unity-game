using UnityEngine;

public class BtnSettingsDropDown : MonoBehaviour
{
    public GameObject saveInterfacePanel;
    public RecentSaveDropDown recentSaveDropDown;

    public void OnClick()
    {
        saveInterfacePanel.LeanScale(Vector3.one, 1);
        recentSaveDropDown.refreshOptions();
    }

    public void Awake()
    {
        saveInterfacePanel.transform.localScale = Vector3.zero;
    }
}
