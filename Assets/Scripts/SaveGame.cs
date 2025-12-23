using UnityEngine;

public class Save : MonoBehaviour
{
    public RecentSaveDropDown dropdown;
    public MultiSaveStrIn SaveStrIn;
    public void SaveManagerData()
    {
        try
        {
            print("Attempting to write to " + SaveStrIn.targetFileName);
            SaveSystem.Save(SaveStrIn.targetFileName);
        }
        catch (System.Exception e)
        {
            print("Failed file save " + e.ToString());
        }

        dropdown.refreshOptions();
    }
}
