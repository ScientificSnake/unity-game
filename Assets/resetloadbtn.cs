using UnityEngine;

public class Resetloadbtn : MonoBehaviour
{
    public MultiSaveStrIn SaveStrIn;
    public void Click()
    {
        SaveSystem.ResetAndLoadDefaults(SaveStrIn.targetFileName);
    }
}
