using System;
using System.Collections.Generic;
using UnityEditor.Analytics;
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

        SysNameToPrefabObj = new Dictionary<string, GameObject>()
        {
            {
            "BasicHullNode",
            Instance.BasicHullBtnPrefab
            }
        };


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TechData.TechCredits = 100;
    }
    #endregion

    // public static int HighestLevelCompleted;

    #region Currect Tech Tree configuration data

    // Note : Node and boon are used interchangeble here and the really mean "Thing you can by off the tech tree", basic boons in the boon pool different.


    #endregion

    #region Level initialization

    public static LevelDataStorage.LevelManager CurrentLevelManagerInstance;

    public void EnterLevel(string sysName)
    {
        Debug.Log("attempting to enter arena scene");

        if (!(LevelDataStorage.LevelDataDict.ContainsKey(sysName)))
        {
            Debug.Log($"Level \" {sysName} \"  could not be found as a a keyin LevelDataStorage.LevelDataDict");
        }
        else // sys name was found
        {
            LevelDataStorage.LevelData targetLevelData = LevelDataStorage.LevelDataDict[sysName];
            CurrentLevelManagerInstance = new LevelDataStorage.LevelManager(targetLevelData, TechData.NodeDataDict);

            LoadingScreen.Instance.Enable();

            // First things first lets move scenes and get that juicy loading screen up
            StartCoroutine(LoadLevelRoutine("arena"));

            //LoadingScreen.Instance.Disable();

            //CurrentLevelManagerInstance.DisplayHullSelectionMenu();

        }
    }

    #region Prefab gameobject fields

    public GameObject BasicHullBtnPrefab;
    // public GameObject FastHullBtnPrefab;
    // public GameObject ScorpionHullBtnPrefab;

    #endregion

    public void SpawnPrefab(GameObject prefab, Vector2 position, Transform parentTransform)
    {
        (Instantiate(prefab, position, Quaternion.identity) as GameObject).transform.parent = parentTransform;
    }


    public Dictionary<string, GameObject> SysNameToPrefabObj;

    private IEnumerator<string> LoadLevelRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        LoadingScreen.Instance.Disable();
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