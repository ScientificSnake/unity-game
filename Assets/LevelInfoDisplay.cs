using TMPro;
using UnityEngine;

public class LevelInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Tmp;
    [SerializeField] private GameObject clickOffBtn;

    private string selectedSysName;

    private bool _shown;

    public void SetBoonInfo(string boonInfoSysName)
    {
        print($"<color=orange> Boon info being set");

        if (!_shown)
        {
            _shown = true;
            gameObject.LeanMoveLocalX(325, 1).setEaseInCubic();

            // allow clicking off
            clickOffBtn.SetActive(true); 
        }
        m_Tmp.text = LevelDataStorage.LevelDataDict[boonInfoSysName].Title;
        selectedSysName = boonInfoSysName;
    }

    public void ClickOffBoonInfo()
    {
        if (_shown)
        {
            _shown = false;
            gameObject.LeanMoveLocalX(625, 1).setEaseInCubic();
            clickOffBtn.SetActive(false);
        }
    }

    public void EnterSelectedMission()
    {
        ManagerScript.Instance.EnterLevel(selectedSysName);
    }
}


