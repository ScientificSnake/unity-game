using UnityEngine;

public class FinishHullSelectionBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.FinishHullSelection();
    }
}
