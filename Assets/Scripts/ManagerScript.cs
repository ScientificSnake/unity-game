using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    #region Global Initialization and singleton
    public static ManagerScript Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    #endregion

    // public static int HighestLevelCompleted;

    #region Currect Tech Tree configuration data

    // Note : Node and boon are used interchangeble here and the really mean "Thing you can by off the tech tree", basic boons in the boon pool different.


    #endregion

    #region Level initialization

    public static void EnterLevel(string sysName)
    {
        if (!(LevelDataStorage.LevelDataDict.ContainsKey(sysName)))
        {
            Debug.Log($"Level \" {sysName} \"  could not be found as a a keyin LevelDataStorage.LevelDataDict");
        }
        else // sys name was found
        {
            // First things first lets move scenes and get that juicy loading screen up

            SceneManager.LoadScene(sceneName: "arena");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reset

            // For loading screen prefab or additively loaded scene? I think panel will work


            LevelDataStorage.LevelData targetLevelData = LevelDataStorage.LevelDataDict[sysName];

            LevelDataStorage.LevelManager currentLevelManager = new (targetLevelData,
                                                                     TechData.NodeDataDict);

        }
    }

    #endregion


    #region Save and Load

    public void Save(ref ManagerSaveData data)
    {
        data.TechCredits = TechData.TechCredits;
        data.NodeDataDict = TechData.NodeDataDict;
    }

    public void Load(ManagerSaveData data)
    {
        TechData.TechCredits = data.TechCredits;
        TechData.NodeDataDict = data.NodeDataDict;

        foreach (var item in data.NodeDataDict)
        {
            string sysName = item.Key;
            bool isPurchased = item.Value.IsNodePurchased;

            if (TechData.NodeDataDict.ContainsKey(sysName))
            {
                TechData.NodeDataDict[sysName].IsNodePurchased = isPurchased;
            }
        }
    }

    #endregion
}

[System.Serializable]

public struct ManagerSaveData
{
    public int TechCredits;
    public List<string> UnlockedBoons;
    public Dictionary<string, TechData.TechNode> NodeDataDict;
}