using TMPro;
using UnityEngine;

public class LevelInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Tmp;
    [SerializeField] private GameObject clickOffBtn;

    private bool _shown;

    public void SetBoonInfo(string boonInfoSysName)
    {
        if (!_shown)
        {
            _shown = true;
            gameObject.LeanMoveX(325, 1).setEaseInCubic();

            // allow clicking off
            clickOffBtn.SetActive(true); 
        }
        m_Tmp.text = LevelDataStorage.LevelDataDict[boonInfoSysName].Title;
    }

    public void ClickOffBoonInfo()
    {
        if (_shown)
        {
            _shown = false;
            gameObject.LeanMoveX(625, 1).setEaseInCubic();
            clickOffBtn.SetActive(false);
        }
    }

    
}
