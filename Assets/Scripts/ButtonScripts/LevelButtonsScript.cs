using UnityEngine;

public class LevelButtonsScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void TestLevel1()
    {
        SendOverLevelInfo("KamiAmbush");
    }
    public void Tutorial()
    {
        SendOverLevelInfo("Tutorial");
    }

    public void ShotGunHell()
    {
        SendOverLevelInfo("ShotgunHell");
    }

    private static LevelInfoDisplay LevelInfoDisplay;

    private void Start()
    {
        LevelInfoDisplay = GameObject.Find("LevelInfoDisplayContainer").GetComponent<LevelInfoDisplay>();
    }

    private void SendOverLevelInfo(string leveSysName)
    {
        LevelInfoDisplay = GameObject.Find("LevelInfoDisplayContainer").GetComponent<LevelInfoDisplay>();
        LevelInfoDisplay.SetBoonInfo(leveSysName);
    }
}
