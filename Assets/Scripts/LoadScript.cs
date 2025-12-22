using UnityEngine;

public class Load : MonoBehaviour
{
    public MultiSaveStrIn SaveStrIn;
    public void LoadManagerData()
    {
        SaveSystem.Load(SaveStrIn.targetFileName);
    }
}
