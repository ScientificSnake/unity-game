using UnityEngine;

public class QuitBtn : MonoBehaviour
{
    public void OnClick()
    {
        ManagerScript.CurrentLevelManagerInstance.GameOver();
    }
}
