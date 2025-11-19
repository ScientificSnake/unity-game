using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class GameDataSL : MonoBehaviour
{
    public static List<string> _UnlockedBoons = new List<string>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void Save(ref GameDataStruct data)
    {
        data.UnlockedBoons = _UnlockedBoons;
    }

    public void Load (GameDataStruct data)
    {

    }
}

[System.Serializable]

public struct GameDataStruct
{
    public List<string> UnlockedBoons;
}



