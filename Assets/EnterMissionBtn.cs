using UnityEngine;

public class EnterMissionBtn : MonoBehaviour
{
    public LevelInfoDisplay levelInfoDisplay;
    
    public void OnClick()
    {
        levelInfoDisplay.EnterSelectedMission();
    }
}
