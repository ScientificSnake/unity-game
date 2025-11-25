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
                "The basic hull type for your spacecraft - John of all Trades \n\nOne could call this the bricks of the PLF Navy Carrier Regiments. The PLF Navy uses this for all kinds of missions, such as in-atmosphere strikes or basic fleet defense.",
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
                "A faster more agile hull with lower base health and more (instability)\n\nIf the Lynchpin is the bricks, then the Swallow is the mortar. The PLF Navy uses this as a basic interdictor fighter, and occasionally they strap fuel pods onto it and use it for escort missions.",
                "SI-290 Swallow",
                new string[] {"BasicHullNode"}   // this some stupid positional argument type shi
                )
        },
        {
            "ScorpionHullNode",
            new TechNode(
                "ScorpionHullNode",
                "HullOption",
                NodeMutationFunctions.ApplyScorpionHull,
                100,
                "An experimental hull with a the new XD-130 drive system, allowing it to dash short distances.\n\nThis one was a spitball project made by the higher ups after Wraith Industries came up with that dash drive system. Very few prototypes were made because of the price, but it's capable enough that it's very rarely used as a defense fighter. However, to be able to house this equipment and keep its maneuverability it is very fragile, so it's only put into the hands of vetaran pilots.",
                "XSF-347 Scorpion",
                new string[] {"SecondHullNode"} // same positional argument tysh
            )
        },
        {
            "TrophyHullNode",
            new TechNode(
                "TrophyHullNode",
                "HullOption",
                NodeMutationFunctions.ApplyTrophyHull,
                100,
                "A cargo hull with the ability to carry 2 light fighter drones, due to an improved control computer and added space.\n\nAfter the [INSERT CIVIL WAR], the higher ups wanted a way to muster more force to cover more area. After the same competition that resulted in the dash drive concept, Pilen Technologies came up with the Enhanced Capability Trophy Upgrade, or ECTU. This allowed the Trophy to carry 2 light fighter drones, give it the ability to act as an extension of carrier wings.",
                "SLC-111C Trophy",
                new string[] {"SecondHullNode"} // positional arg tysh
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
            Debug.Log("test applied scorpion hull");
        }
        public static void ApplyTrophyHull()
        {
            Debug.Log("test applied trophy hull");
        }
    #endregion
    }
}
