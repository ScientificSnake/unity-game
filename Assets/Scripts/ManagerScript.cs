using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            },
            {
                "SecondHullNode",
                Instance.FastHullBtnPrefab
            },
            {
                "ScorpionHullNode",
                Instance.ScorpionHullBtnPrefab
            },
            {
                "TrophyHullNode",
                Instance.TrophyHullBtnPrefab
            },
            {
                "RavenHullNode",
                Instance.RavenHullBtnPrefab
            }
        };

        SpriteDict = new Dictionary<string, Sprite>()
        {
            {
                "BasicHullNode",
                Instance.LynchpinSprite
            },
            {
                "SecondHullNode",
                Instance.SwallowSprite
            },
            {
                "ScorpionHullNode",
                Instance.ScorpionSprite
            },
            {
                "TrophyHullNode",
                Instance.TrohpySprite
            },
            {
                "RavenHullNode",
                Instance.RavenSprite
            }
        };

    }
    void Start()
    {
        TechData.TechCredits = 500;
        TechData.HullOptionsDataDict["BasicHullNode"].IsNodePurchased = true; // start with a basic hull
    }
    #endregion
    #region Currect Tech Tree configuration data

    // Note : Node and boon are used interchangeble here and the really mean "Thing you can buy off the tech tree", basic boons in the boon pool different.


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
            CurrentLevelManagerInstance = new LevelDataStorage.LevelManager(targetLevelData, TechData.HullOptionsDataDict);

            LoadingScreen.Instance.Enable();

            // First things first lets move scenes and get that juicy loading screen up
            StartCoroutine(LoadSceneRoutine("hullSelection"));

            //LoadingScreen.Instance.Disable();

            //CurrentLevelManagerInstance.DisplayHullSelectionMenu();

        }
    }
    #endregion
    #region Prefab gameobject fields

    public GameObject BasicHullBtnPrefab;
    public GameObject FastHullBtnPrefab;
    public GameObject ScorpionHullBtnPrefab;
    public GameObject RavenHullBtnPrefab;
    public GameObject TrophyHullBtnPrefab;

    public GameObject CarrierSpritePrefab;
    public GameObject BasicHullSpritePrefab;
    public GameObject PlayerHullObjPrefab;
    public GameObject BasicBulletPrefab;
    public GameObject RocketPrefab;

    #endregion

    #region Sprite game object fields
    public Sprite LynchpinSprite;
    public Sprite ScorpionSprite;
    public Sprite SwallowSprite;
    public Sprite TrohpySprite;
    public Sprite RavenSprite;

    public Sprite LynchpinThrusting;
    public Sprite ScorpionThrusting;
    public Sprite SwallowThrusting;
    public Sprite TrophyThrusting;
    public Sprite RavenThrusting;
    #endregion

    #region Level Layout fields

    public GameObject TutorialLayoutPrefab;

    #endregion

    public Dictionary<string, GameObject> SysNameToPrefabObj;

    public Dictionary<string, Sprite> SpriteDict;

    public void ChangeBackground(Image Canvas, Sprite Background)
    {
        Canvas.sprite = Background;
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector2 position, Transform parentTransform)
    {
        GameObject newGameObj = Instantiate(prefab, position, Quaternion.identity);
        newGameObj.transform.SetParent(parentTransform, false);
        return newGameObj;
    }

    public GameObject SpawnOrphan(GameObject prefab, Vector2 position)
    {
        GameObject newGameObj = Instantiate(prefab, position, Quaternion.identity);
        return newGameObj;
    }

    public void FinishHullSelection()
    {
        try
        {
            string targetSysName = CurrentLevelManagerInstance.selectedHull;

            print($"Target sysname is {targetSysName}");

            Action<LevelDataStorage.LevelManager.BaseStats> targetMutationFunc = TechData.HullOptionsDataDict[targetSysName].MutationFunc;

            // populate BaseStats Dictionary 
            targetMutationFunc(CurrentLevelManagerInstance.Stats);

            StartCoroutine(LoadSceneRoutine("Arena"));
        }
        catch (Exception e)
        {
            Debug.Log("you probably need to actually click a hull option dumbass");
            Debug.Log(e.ToString());
        }
    }

    public IEnumerator StartLastEnemySpawnTimer(int timeSeconds)
    {
        yield return new WaitForSeconds(timeSeconds);
        CurrentLevelManagerInstance.LastEnemySpawned = true;
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        LoadingScreen.Instance.Disable();
    }

    /// <summary>
    /// Returns how many objects with a certain tag remain in the scene
    /// </summary>
    /// <returns></returns>
    public int HowManyObjectsWithTag(string Tag)
    {
        return GameObject.FindGameObjectsWithTag(Tag).Length;
    }


    #region Save and Load

    public void Save(ref ManagerSaveData data)
    {
        data.TechCredits = TechData.TechCredits;

        // Only save which nodes are purchased
        data.PurchasedNodes = new Dictionary<string, bool>();
        foreach (var item in TechData.HullOptionsDataDict)
        {
            data.PurchasedNodes[item.Key] = item.Value.IsNodePurchased;
        }
    }

    public void Load(ManagerSaveData data)
    {
        TechData.TechCredits = data.TechCredits;

        // Just restore the purchased status - the dictionary already exists with all the delegates intact
        foreach (var item in data.PurchasedNodes)
        {
            if (TechData.HullOptionsDataDict.ContainsKey(item.Key))
            {
                TechData.HullOptionsDataDict[item.Key].IsNodePurchased = item.Value;
            }
        }
    }

    #endregion
}

[System.Serializable]
public struct ManagerSaveData
{
    public int TechCredits;
    public Dictionary<string, bool> PurchasedNodes;
}