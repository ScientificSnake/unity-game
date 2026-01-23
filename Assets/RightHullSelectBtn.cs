using UnityEngine;

public class RightHullSelectBtn : MonoBehaviour
{
    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.ShiftHullSelectionBtns(LR.Right);
    }
}
