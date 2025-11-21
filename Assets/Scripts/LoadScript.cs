using UnityEngine;

public class Load : MonoBehaviour
{
    public void LoadManagerData()
    {
        print("Loading Data");
        SaveSystem.Load();
    }
}
