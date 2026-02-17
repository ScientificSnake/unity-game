using TMPro;
using UnityEngine;

public class LevelInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Tmp;
    [SerializeField] private GameObject clickOffBtn;
    [SerializeField] private GameObject prevLevelsNotMetWarning;

    private string selectedSysName;

    private bool _shown;

    public void SetBoonInfo(string boonInfoSysName)
    {
        print($"<color=orange> Boon info being set");

        if (!_shown)
        {
            void UpdateState()
            {
                _shown = true;
            }

            ObjTools.RunOnDelay(this, UpdateState, 0.7f);

            gameObject.LeanMoveLocalX(325, 0.7f).setEaseInExpo();

            // allow clicking off
            clickOffBtn.SetActive(true); 
        }
        m_Tmp.text = LevelDataStorage.LevelDataDict[boonInfoSysName].Title;
        selectedSysName = boonInfoSysName;

        if (DependentLevelsAreMet())
        {
            prevLevelsNotMetWarning.SetActive(false);
        }
        else
        {
            prevLevelsNotMetWarning.SetActive(true);
        }
    }

    private bool DependentLevelsAreMet()
    {
        foreach(string level in LevelDataStorage.LevelDataDict[selectedSysName].NeededBeatenLevels)
        {
            if (!ManagerScript.Instance.BeatenLevels.Contains(level))
            {
                return false;
            }
        }
        return true;
    }

    public void ClickOffBoonInfo()
    {
        if (_shown)
        {
            clickOffBtn.SetActive(false);
            void UpdateState()
            {
                _shown = false;
            }

            ObjTools.RunOnDelay(this, UpdateState, 0.7f);

            gameObject.LeanMoveLocalX(625, 0.7f).setEaseInExpo();
        }
    }

    public void EnterSelectedMission()
    {
        ManagerScript.Instance.EnterLevel(selectedSysName);
    }
}


