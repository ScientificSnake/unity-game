using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable] // System.Serializable is not strictly needed for Newtonsoft but can be kept
    public struct SaveData
    {
        public ManagerSaveData ManagerData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".xanxan";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        Debug.Log($"Writing to {SaveFileName()}");

        // Use JsonConvert.SerializeObject instead of JsonUtility.ToJson
        // 'Formatting.Indented' makes the file easy to read
        File.WriteAllText(SaveFileName(), JsonConvert.SerializeObject(_saveData, Formatting.Indented));
    }

    private static void HandleSaveData()
    {
        // This line remains the same, it just prepares the struct
        ManagerScript.Instance.Save(ref _saveData.ManagerData); 
    }

    public static void Load()
    {
        Debug.Log($"Loading file from {SaveFileName()}");

        // Add a check to make sure the file exists before trying to read it
        if (!File.Exists(SaveFileName()))
        {
            Debug.LogWarning("Save file does not exist, cannot load.");
            return;
        }

        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonConvert.DeserializeObject<SaveData>(saveContent);
        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        Debug.Log("setting manager data now");
        ManagerScript.Instance.Load(_saveData.ManagerData);
    }
}
