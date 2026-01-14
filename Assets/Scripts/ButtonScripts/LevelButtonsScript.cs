using UnityEngine;

public class LevelButtonsScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void TestLevel1()
    {
        SendOverLevelInfo("TestLevel1");
    }
    public void Tutorial()
    {
        SendOverLevelInfo("Tutorial");
    }

    private static LevelInfoDisplay LevelInfoDisplay;

    private void Awake()
    {
        if (LevelInfoDisplay == null)
        {
            LevelInfoDisplay = GameObject.Find("LevelInfoDisplay").GetComponent<LevelInfoDisplay>();
        }
    }
    private void SendOverLevelInfo(string leveSysName)
    {
        LevelInfoDisplay.SetBoonInfo(leveSysName);
    }
}
