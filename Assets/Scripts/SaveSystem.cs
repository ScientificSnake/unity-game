using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Unity.Collections;
using TMPro;

public class SaveSystem
{
    private static SaveData _saveData = new();
    private static MetaSaveData _metaSaveData = new();

    [System.Serializable] // System.Serializable is not strictly needed for Newtonsoft but can be kept
    public struct SaveData
    {
        public ManagerSaveData ManagerData;
    }

    public static string MetaSaveFileName()
    {
        string metaSaveFile = Application.persistentDataPath + "/" + "metadata" + ".metaxanxan";
        return metaSaveFile;
    }

    public static string SaveFileName(string filename)
    {
        string saveFile = Application.persistentDataPath + "/" + filename + ".xanxan";
        return saveFile;
    }

    public static void Save(string filename)
    {
        HandleSaveData();

        Debug.Log($"Writing to {SaveFileName(filename)}");

        // Use JsonConvert.SerializeObject instead of JsonUtility.ToJson
        // 'Formatting.Indented' makes the file easy to read
        File.WriteAllText(SaveFileName(filename), JsonConvert.SerializeObject(_saveData, Formatting.Indented));

        ManagerScript.Instance.LastSaveFileName = filename;

        SaveMeta();

        ManagerScript.Instance.RecentFiles.Enqueue(filename);
    }

    private static void HandleSaveData()
    {
        // This line remains the same, it just prepares the struct
        ManagerScript.Instance.Save(ref _saveData.ManagerData); 
    }

    public static void HandleSaveMetaData()
    {
        ManagerScript.Instance.SaveMeta(ref _metaSaveData);
    }

    public static void SaveMeta()
    {
        HandleSaveMetaData();
        File.WriteAllText(MetaSaveFileName(), JsonConvert.SerializeObject(_metaSaveData, Formatting.Indented));
    }

    public static void Load(string filename)
    {
        Debug.Log($"Loading file from {SaveFileName(filename)}");

        // Add a check to make sure the file exists before trying to read it
        if (!File.Exists(SaveFileName(filename)))
        {
            Debug.LogWarning("Save file does not exist, cannot load.");
            return;
        }

        string saveContent = File.ReadAllText(SaveFileName(filename));

        _saveData = JsonConvert.DeserializeObject<SaveData>(saveContent);
        HandleLoadData();
        ManagerScript.Instance.RecentFiles.Enqueue(filename);
    }

    public static void LoadMeta()
    {
        Debug.Log($"Loading meta dat from {MetaSaveFileName()}");

        if (!File.Exists(MetaSaveFileName())){
            Debug.LogWarning("Meta save file does not exist; ignoring");
            throw new System.Exception("No meta save data found");
        }

        string metaSaveContent = File.ReadAllText(MetaSaveFileName());

        _metaSaveData = JsonConvert.DeserializeObject<MetaSaveData>(metaSaveContent);

        HandleLoadMetaData();
    }

    public static void HandleLoadData()
    {
        Debug.Log("setting manager data now");
        ManagerScript.Instance.Load(_saveData.ManagerData);
    }

    public static void HandleLoadMetaData()
    {
        Debug.Log("Setting manager meta data now");
        {
            ManagerScript.Instance.LoadMeta(_metaSaveData);
        }
    }

    public static void ResetAndLoadDefaults(string filename)
    {
        try
        {
            TechData.TechCredits = 500;
            foreach (var item in TechData.HullOptionsDataDict)
            {
                item.Value.IsNodePurchased = false;
            }

            TechData.HullOptionsDataDict["LynchpinHullNode"].IsNodePurchased = true;

            Save(filename);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to reset and load save file: {e.Message}");
        }
    }
}
