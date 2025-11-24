using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TechData : MonoBehaviour
{
    public static int TechCredits;

    public static bool IsNodePurchased(string sysname)
    {
        return NodeDataDict[sysname].IsNodePurchased;
    }
    public static void PurchaseNode(string sysname)
    {
        NodeDataDict[sysname].IsNodePurchased = true;
    }


    public static bool AreNodeDependciesMet(string sysName)
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
                "SI-290 Swallow",
                new string[] {"BasicHullNode"}   // this some stupid positional argument type shi
                )
        },
        {
            "ScorpionHullNode",
            new TechNode(
                "ScorpionHullNode",
                "HullOption",
                NodeMutationFunctions.ApplySecondHull,
                100,
                "An experimental hull with a the new XD-130 drive system, allowing it to dash short distances",
                "XSF-347 Scorpion",
                new string[] {"SecondHullNode"} // same positional argument tysh
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

        public static void ApplyScorpionHull()
        {
            Debug.Log("test applied second hull");
        }
    #endregion
    }
}
