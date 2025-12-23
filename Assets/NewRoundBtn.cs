using UnityEngine;

public class NewRoundBtn : MonoBehaviour
{
    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
    }
}
