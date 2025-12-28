using UnityEngine;

public class NewRoundBtn : MonoBehaviour
{
    public void OnClick()
    {
        print($"<color=yellow> New round button clicked");
        ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
    }
}
