using UnityEngine;

public class Save : MonoBehaviour
{
    public void SaveManagerData()
    {
        print("Saving Data");
        SaveSystem.Save();
    }
}
