using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TechData : MonoBehaviour
{
    #region global initialization and singleton set up
    public static TechData Instance { get; private set; }
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
    void Start()
    {
        TechCredits = 100;
    }

    #endregion
    public static int TechCredits;

    public bool IsNodePurchased(string sysname)
    {
        return NodeDataDict[sysname].IsNodePurchased;
    }
    public void PurchaseNode(string sysname)
    {
        NodeDataDict[sysname].IsNodePurchased = true;
    }


    public bool AreNodeDependciesMet(string sysName)
    {
        Array dependencies = TechData.NodeDataDict[sysName].DependencyNodes;

        foreach (string dependency in dependencies)
        {
            if ((IsNodePurchased(dependency) is false))
            {
                return false;
            }
        }
        return true;
    }

    #region Node Data
    [System.Serializable]
    public class TechNode
    {
        public string SysName { get; }
        public string NodeType { get; }
        public bool IsNodePurchased { get; set; }

        [JsonIgnore]
        public Action MutationFunc { get; }


        public int Price { get; }
        public string[] DependencyNodes { get; }
        public string DisplayText { get; }
        public string DisplayTitle { get; }

        public TechNode(string sysName, string nodeType, Action mutationFunc, int price, string displayText, string displayTitle, string[] dependencyNodes = null)
        {
            SysName = sysName;
            NodeType = nodeType;
            MutationFunc = mutationFunc;
            Price = price;
            DisplayText = displayText;
            DisplayTitle = displayTitle;

            DependencyNodes = dependencyNodes ?? Array.Empty<string>();

            IsNodePurchased = false;
        }
    }

    public static Dictionary<string, TechNode> NodeDataDict = new()
    {
        {
            "BasicHullNode",
            new TechNode(
                "BasicHullNode",
                "HullOption",
                NodeMutationFunctions.ApplyBasicHull,
                30,
                "The Basic Hull Type for your spacecraft - John of all Trades",
                "SF-170 Lynchpin"
                )
        },
        {
            "SecondHullNode",
            new TechNode(
                "SecondHullNode",
                "HullOption",
                NodeMutationFunctions.ApplySecondHull,
                67,
                "A faster more agile hull with lower base health and more (instability)",
                "SI-290 Swallow"
                )
        }
    };
    #endregion
    #region Node functions for mutation for mutation of level managers data
    [System.Serializable]
    public static class NodeMutationFunctions
    {
        public static void ApplyBasicHull()
        {
            Debug.Log("Test applied basic hull");
        }

        public static void ApplySecondHull()
        {
            Debug.Log("test applied second hull");
        }
    #endregion
    }
}
