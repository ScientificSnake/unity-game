using UnityEngine;

public class HullOptionBtnScriptTemplate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string HullOptionSysName;
    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.selectedHull = HullOptionSysName;
    }
}
