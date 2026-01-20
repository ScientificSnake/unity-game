using UnityEngine;

public class ClickOffBoonInfoScript : MonoBehaviour
{

    private LevelInfoDisplay levelInfoDisplay;

    private void Awake()
    {
        levelInfoDisplay = GameObject.Find("LevelInfoDisplayContainer").GetComponent<LevelInfoDisplay>();
    }
    public void ClickOff()
    {
        levelInfoDisplay.ClickOffBoonInfo();
    }
}
