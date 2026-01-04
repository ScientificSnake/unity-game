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
            BoonAction();
            print($"<color=yellow> New round button clicked");
            ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
        }
        else
        {
            warning.SendWarn();
        }
    }
}
