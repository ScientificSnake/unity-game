using UnityEngine;

public class LevelButtonsScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void TestLevel1()
    {
        ManagerScript.Instance.EnterLevel("TestLevel1");
    }
    public void Tutorial()
    {
        ManagerScript.Instance.EnterLevel("Tutorial");
    }
}
