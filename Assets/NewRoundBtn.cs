using System;
using UnityEngine;

public class NewRoundBtn : MonoBehaviour
{
    public Action BoonAction;

    [SerializeField] public WarningFromAbove warning;

    public void OnClick()
    {
        if (BoonAction != null)
        {
            Debug.Log($"<color=yellow> New round button clicked");
            //BoonAction();
            ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
        }
        else
        {
            warning.SendWarn();
        }
    }
}
