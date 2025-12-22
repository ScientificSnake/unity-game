using UnityEngine;

public class Load : MonoBehaviour
{
    public MultiSaveStrIn SaveStrIn;
    public CurrentLoadTextUpdater CurrentLoadTextUpdater;
    public void LoadManagerData()
    {
        SaveSystem.Load(SaveStrIn.targetFileName);
        ManagerScript.Instance.CurrentLoad = SaveStrIn.targetFileName;
        CurrentLoadTextUpdater.UpdateText();

        ManagerScript.Instance.LastLoadName = SaveStrIn.targetFileName;

        SaveSystem.SaveMeta();
    }
}
