using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            return;
        }

        #region Populating dicts for references
        SysNameToPrefabObj = new Dictionary<string, GameObject>()
        {
            {
                "LynchpinHullNode",
                Instance.BasicHullBtnPrefab
            },
            {
                "SwallowHullNode",
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
                "LynchpinHullNode",
                Instance.LynchpinSprite
            },
            {
                "SwallowHullNode",
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
        #endregion

        #region Load meta save data
        try
        {
            SaveSystem.LoadMeta();

            SaveSystem.Load(LastSaveFileName);
            ManagerScript.Instance.CurrentLoad = ManagerScript.Instance.LastLoadName;
        }
        catch (Exception e)
        {
            print(e);
            Debug.Log("Loading default save state");
            TechData.TechCredits = 500;
            TechData.HullOptionsDataDict["LynchpinHullNode"].IsNodePurchased = true; // start with a basic hull
        }
    }
        #endregion
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

    public GameObject BasicThrusterPrefab;

    #endregion

    #region Sprite game object fields
    public Sprite LynchpinSprite;
    public Sprite ScorpionSprite;
    public Sprite SwallowSprite;
    public Sprite TrohpySprite;
    public Sprite RavenSprite;
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

    public string CurrentLoad;

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

    public IEnumerator FadeInSprite(SpriteRenderer targetSprite, float totalFadeTime)
    {
        Color color = targetSprite.color;
        int totalFadeSteps = Mathf.FloorToInt(totalFadeTime / Time.fixedDeltaTime);
        float increment = (1f / (float)totalFadeSteps);
        for (int i = 0; i < (totalFadeSteps - 1); i++)
        {
            color.a += increment;
            targetSprite.color = color;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        color.a = 1;
        targetSprite.color = color;
    }

    /// <summary>
    /// Returns how many objects with a certain tag remain in the scene
    /// </summary>
    /// <returns></returns>
    public int HowManyObjectsWithTag(string Tag)
    {
        return GameObject.FindGameObjectsWithTag(Tag).Length;
    }
    public void UpdateNodes()
    {
        // grab a ref to canvas
        GameObject container = GameObject.Find("NodeTreeContainer");

        List<NodeCapsuleScript> childNodeScripts = new();

        foreach (Transform childTransform in container.transform)
        {
            childNodeScripts.Add(childTransform.gameObject.GetComponent<NodeCapsuleScript>());
        }

        foreach (NodeCapsuleScript script in childNodeScripts)
        {
            if (TechData.IsNodePurchased(script.SysName))
            {
                Color orangeBgColor = script.OrangeBg.GetComponent<Image>().color;
                orangeBgColor.a = 1;
                script.OrangeBg.GetComponent<Image>().color = orangeBgColor;

                Color btnCompcolor = script.BtnComp.GetComponent<Image>().color;
                btnCompcolor.r = 1;
                btnCompcolor.g = 1;
                btnCompcolor.b = 1;
                script.BtnComp.GetComponent<Image>().color = btnCompcolor;
            }
            else
            {
                Color orangeBgColor = script.OrangeBg.GetComponent<Image>().color;
                orangeBgColor.a = 0;
                script.OrangeBg.GetComponent<Image>().color = orangeBgColor;

                if (TechData.AreNodeDependciesMet(script.SysName))
                {
                    Color btnCompcolor = script.BtnComp.GetComponent<Image>().color;
                    btnCompcolor.r = 1;
                    btnCompcolor.g = 1;
                    btnCompcolor.b = 1;
                    script.BtnComp.GetComponent<Image>().color = btnCompcolor;
                }
                else
                {
                    Color btnCompcolor = script.BtnComp.GetComponent<Image>().color;
                    btnCompcolor.r = 0.35f;
                    btnCompcolor.g = 0.35f;
                    btnCompcolor.b = 0.35f;
                    script.BtnComp.GetComponent<Image>().color = btnCompcolor;
                }
            }
        }
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

    public string LastLoadName;
    public string LastSaveFileName;

    public void SaveMeta(ref MetaSaveData data)
    {
        data.LastLoadFileName = LastLoadName;
        data.LastSaveFileName = LastSaveFileName;
        data.RecentFiles = RecentFiles.ToList();

        Debug.Log($"Saving meta data with {LastLoadName} as last load name and {LastSaveFileName} as last save name");  
    }

    public void LoadMeta(MetaSaveData data)
    {
        LastLoadName = data.LastLoadFileName;
        LastSaveFileName = data.LastSaveFileName;


        foreach (var item in data.RecentFiles)
        {
            RecentFiles.Enqueue(item);
        }
        Debug.Log($"Meta load yields {LastLoadName} and {LastSaveFileName} for load and save respectively)");
    }
    #endregion

    public FixedSizeQueue<string> RecentFiles = new(4);
}

[System.Serializable]
public struct ManagerSaveData
{
    public int TechCredits;
    public Dictionary<string, bool> PurchasedNodes;
}

public struct MetaSaveData
{
    public string LastLoadFileName;
    public string LastSaveFileName;
    public List<string> RecentFiles;
}