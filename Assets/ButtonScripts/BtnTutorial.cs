using UnityEngine;

public class BtnTutorial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnClick()
    {
        ManagerScript.Instance.EnterLevel("Tutorial");
    }
}
