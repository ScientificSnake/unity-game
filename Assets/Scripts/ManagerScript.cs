using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript Instance { get; private set; }

    // public static int HighestLevelCompleted;

    #region Currect Tech Tree configuration data

    // Note : Node and boon are used interchangeble here and the really mean "Thing you can by off the tech tree", basic boons in the boon pool different.

    public static List<string> UnlockedBoons = new List<string>();
    public static int TechCredits;

    public static Dictionary<string, bool> NodePurchaseLog = new Dictionary<string, bool>  // THIS INCLUDES ALL THE Nodes AND WHETHER THEY HAVE BEEN PURCHASED OR NOT
    {
        { "TestBoon1", false},
        { "TestNode2", false}
    };

    public bool IsNodePurchased(string sysname)
    {
        return NodePurchaseLog[sysname];
    }
    public void PurchaseNode(string sysname)
    {
        NodePurchaseLog[sysname] = true;
    }


    public bool AreNodeDependciesMet(List<string> Dependencies)
    {
        foreach (string Dependency in Dependencies)
        {
            if ((IsNodePurchased(Dependency) is false))
            {
                return false;
            }
        }
        return true;
    }


    #endregion



    #region Node Data

    public interface TechNode
    {
        public string sysName { get; set; }
        public string nodeType { get; set; }

        public bool isNodePurchased { get; set; } // Can migrate to this from Nodelog

    }


    # endregion

    #region Global Initialization

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TechCredits = 100;
    }

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
    #endregion



    #region Save and Load

    public void Save(ref ManagerSaveData data)
    {
        data.TechCredits = TechCredits;
        data.NodePurchaseLog = NodePurchaseLog;
        data.UnlockedBoons = UnlockedBoons;
    }

    public void Load(ManagerSaveData data)
    {
        TechCredits = data.TechCredits;
        NodePurchaseLog = data.NodePurchaseLog;
        UnlockedBoons = data.UnlockedBoons;

        print(NodePurchaseLog.ToString());
    }

    #endregion
}

[System.Serializable]

public struct ManagerSaveData
{
    public int TechCredits;
    public List<string> UnlockedBoons;
    public Dictionary<string, bool> NodePurchaseLog;
}