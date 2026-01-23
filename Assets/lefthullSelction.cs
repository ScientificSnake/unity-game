using UnityEngine;

public class lefthullSelction : MonoBehaviour
{
    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.ShiftHullSelectionBtns(LR.Left);
    }
}