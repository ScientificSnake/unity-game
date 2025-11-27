using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class TechData : MonoBehaviour
{
    public static int TechCredits;

    public static bool IsNodePurchased(string sysname)
    {
        return HullOptionsDataDict[sysname].IsNodePurchased;
    }
    public static void PurchaseNode(string sysname)
    {
        HullOptionsDataDict[sysname].IsNodePurchased = true;
    }


    public static bool AreNodeDependciesMet(string sysName)
    {
        Array dependencies = TechData.HullOptionsDataDict[sysName].DependencyNodes;

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
        public Action<object> MutationFunc { get; }


        public int Price { get; }
        public string[] DependencyNodes { get; }
        public string DisplayText { get; }
        public string DisplayTitle { get; }

        public TechNode(string sysName, string nodeType, Action<object> mutationFunc, int price, string displayText, string displayTitle, string[] dependencyNodes = null)
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

    public static Dictionary<string, TechNode> HullOptionsDataDict = new()
    {
        {
            "BasicHullNode",
            new TechNode(
                "BasicHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyBasicHull,
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
                HullStatsMutationFunctions.ApplySecondHull,
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
                HullStatsMutationFunctions.ApplyScorpionHull,
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
                HullStatsMutationFunctions.ApplyTrophyHull,
                125,
                "A cargo hull with the ability to carry 2 light fighter drones, due to an improved control computer and added space.\n\nAfter the [INSERT CIVIL WAR], the higher ups wanted a way to muster more force to cover more area. After the same competition that resulted in the dash drive concept, Pilen Technologies came up with the Enhanced Capability Trophy Upgrade, or ECTU. This allowed the Trophy to carry 2 light fighter drones, give it the ability to act as an extension of carrier wings.",
                "SLC-111C Trophy",
                new string[] {"SecondHullNode"} // positional arg tysh
            )
        },
        {
            "RavenHullNode",
            new TechNode(
                "RavenHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyRavenHull,
                150,
                "A fighter equipped with a cloaking pod that allows it to deceive the enemy for a limited period of time.\n\nSome of the scientists at Wraith Industries came up with this cloaking tech after putting a part from their dash drive in the microwave \"for science\". At the same time they were prototyping a multirole capable of operating both in and out of the atmosphere with minimal changes, so the techs decided to merge the tech and make the Raven. The general public, me included, thought that it would be too expensive to go into full production, but the higher ups saw the potential in it and gave Wraith a contract to produce it at the same scale as the Lynchpin.",
                "SFS-362Raven",
                new string[] {"TrophyHullNode", "ScorpionHullNode"} // positional arg tysh
            )
        }
    };
    #endregion
    #region Node functions for mutation of base stats
    [System.Serializable]
    private static class HullStatsMutationFunctions
    {
        public static void ApplyBasicHull(object BaseStatsObj)  // Populating the dictionary of base stats
        {
            Debug.Log("Test applied basic hull");

            if (BaseStatsObj is Dictionary<string, int> BaseStats)  // OH YEAH REFERENCE MUTATION
            {
                BaseStats["Health"] = 100; // yeah no we ain't doin no fucking module health
                BaseStats["MaxTurnRate"] = 36; // degress per second 
                BaseStats["Acceleration"] = 20;  // SUBJECT TO FURTHER CHANGE pixels per second^2
            }

        }

        public static void ApplySecondHull(object BaseStatsObj)
        {
            Debug.Log("test applied second hull");
            if (BaseStatsObj is Dictionary<string, int> BaseStats)
            {
                print("detected dict object");
            }
            else { print("argument not dictionary<string, int>"); }
        }

        public static void ApplyScorpionHull(object BaseStatsObj)
        {
            Debug.Log("test applied scorpion hull");
            if (BaseStatsObj is Dictionary<string, int> BaseStats)
            {
                print("detected dict objec");
            }
        }
        public static void ApplyTrophyHull(object BaseStatsObj)
        {
            Debug.Log("test applied trophy hull");

            if (BaseStatsObj is Dictionary<string, int> BaseStats)
            {
                print("detected dict object");
            }
        }
        public static void ApplyRavenHull(object BaseStatsObj)
        {
            Debug.Log("test applied raven hull");
        }
        public static void ApplyRavenHull()
        {
            Debug.Log("test applied raven hull");
        }
    #endregion
    }
}
