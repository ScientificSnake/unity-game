using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

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


    public delegate void HullMutation(ref LevelDataStorage.LevelManager.BaseStats stats);
    #region Node Data
    [System.Serializable]
    public class HullNode : ITechNode
    {
        public string SysName { get; }
        public string NodeType { get; }
        public bool IsNodePurchased { get; set; }

        [JsonIgnore]
        public HullMutation MutationFunc { get; }

        public int Price { get; }
        public string[] DependencyNodes { get; }
        public string DisplayText { get; }
        public string DisplayTitle { get; }

        public HullNode(string sysName, string nodeType, HullMutation mutationFunc, int price, string displayText, string displayTitle, string[] dependencyNodes = null)
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

    public static Dictionary<string, HullNode> HullOptionsDataDict = new()
    {
        {
            "LynchpinHullNode",
            new HullNode(
                "LynchpinHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyBasicHull,
                30,
                "The basic hull type for your spacecraft - John of all Trades \n\nSTATS:\nHealth: 100\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: None\n\nOne could call this the bricks of the PLF Navy Carrier Regiments. The PLF Navy uses this for all kinds of missions, such as in-atmosphere strikes or basic fleet defense.",
                "SF-170 Lynchpin"
                )
        },
        {
            "SwallowHullNode",
            new HullNode(
                "SwallowHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplySecondHull,
                70,
                "A faster more agile hull with lower base health and more (instability)\n\nSTATS:\nHealth: 85\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: None\n\nIf the Lynchpin is the bricks, then the Swallow is the mortar. The PLF Navy uses this as a basic interdictor fighter, and occasionally they strap fuel pods onto it and use it for escort missions.",
                "SI-290 Swallow",
                new string[] {"LynchpinHullNode"}   // this some stupid positional argument type shi
                )
        },
        {
            "ScorpionHullNode",
            new HullNode(
                "ScorpionHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyScorpionHull,
                100,
                "An experimental hull with a the new XD-130 drive system, allowing it to dash short distances.\n\nSTATS:\nHealth: 75\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Dash\n\nThis one was a spitball project made by the higher ups after Wraith Industries came up with that dash drive system. Very few prototypes were made because of the price, but it's capable enough that it's very rarely used as a defense fighter. However, to be able to house this equipment and keep its maneuverability it is very fragile, so it's only put into the hands of vetaran pilots.",
                "XSF-347 Scorpion",
                new string[] {"SwallowHullNode"} // same positional argument tysh
            )
        },
        {
            "TrophyHullNode",
            new HullNode(
                "TrophyHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyTrophyHull,
                125,
                "A cargo hull with the ability to carry 2 light fighter drones, due to an improved control computer and added space.\n\nSTATS:\nHealth: 110\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Drone Carrying\n\nAfter the [INSERT CIVIL WAR], the higher ups wanted a way to muster more force to cover more area. After the same competition that resulted in the dash drive concept, Pilen Technologies came up with the Enhanced Capability Trophy Upgrade, or ECTU. This allowed the Trophy to carry 2 light fighter drones, give it the ability to act as an extension of carrier wings.",
                "SLC-111C Trophy",
                new string[] {"SwallowHullNode"} // positional arg tysh
            )
        },
        {
            "RavenHullNode",
            new HullNode(
                "RavenHullNode",
                "HullOption",
                HullStatsMutationFunctions.ApplyRavenHull,
                150,
                "A fighter equipped with a cloaking pod that allows it to deceive the enemy for a limited period of time.\n\nSTATS:\nHealth: 100\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Cloaking\n\nSome of the scientists at Wraith Industries came up with this cloaking tech after putting a part from their dash drive in the microwave \"for science\". At the same time they were prototyping a multirole capable of operating both in and out of the atmosphere with minimal changes, so the techs decided to merge the tech and make the Raven. The general public, me included, thought that it would be too expensive to go into full production, but the higher ups saw the potential in it and gave Wraith a contract to produce it at the same scale as the Lynchpin.",
                "SFS-362Raven",
                new string[] {"TrophyHullNode", "ScorpionHullNode"} // positional arg tysh
            )
        }
    };
    #endregion
    private static class HullStatsMutationFunctions//RULES TO MAKE IT FEEL GOOD- REDUCE THRUST=REDUCE TURN RATE ADD THRUST=ADD TURN RATE
    {
        public static void ApplyBasicHull(ref LevelDataStorage.LevelManager.BaseStats BaseStats)
        {
            Debug.Log("Test applied basic hull");

            BaseStats.Health = 1000;
            BaseStats.MaxTurnRate = 107;
            BaseStats.Acceleration = 5000;
            BaseStats.Mass = 2.5f;
            BaseStats.ScaleFactor = 1.75f;
            BaseStats.GunOffset = new Vector2(10, 0);
            BaseStats.BaseFuel = 60;
            BaseStats.thrusterLayout = Thrusters.LynchpinThrusterSet;
            BaseStats.StartingAmmoCount = 1500;

            BaseStats.DashAbility = false;
            BaseStats.StealthAbility = false;
            BaseStats.CarrierAbility = false;
            BaseStats.WeaponSelection = 1;
            BaseStats.SelectedWeapon = Sebastian.WeaponryData.WeaponDict[1];
        }

        public static void ApplySecondHull(ref LevelDataStorage.LevelManager.BaseStats BaseStats)
        {
            Debug.Log("test applied second hull");

            BaseStats.Health = 850;
            BaseStats.MaxTurnRate = 112;
            BaseStats.Acceleration = 5200;
            BaseStats.Mass = 1.75f;
            BaseStats.ScaleFactor = 1.9f;
            BaseStats.BaseFuel = 50;
            BaseStats.GunOffset = new Vector2(7, -1.1f);
            BaseStats.thrusterLayout = Thrusters.SwallowThrusterSet;
            BaseStats.StartingAmmoCount = 1150;

            BaseStats.DashAbility = false;
            BaseStats.StealthAbility = false;
            BaseStats.CarrierAbility = false;
            BaseStats.WeaponSelection = 1;
        }

        public static void ApplyScorpionHull(ref LevelDataStorage.LevelManager.BaseStats BaseStats)
        {
            Debug.Log("test applied scorpion hull");

            BaseStats.Health = 750;
            BaseStats.MaxTurnRate = 130;
            BaseStats.Acceleration = 6000;
            BaseStats.ReverseAcceleration = 10;
            BaseStats.Mass = 1.5f;
            BaseStats.ScaleFactor = 1.25f;
            BaseStats.BaseFuel = 50;
            BaseStats.GunOffset = new Vector2(15, 0.7f);
            BaseStats.thrusterLayout = Thrusters.ScorpionThrusterSet;
            BaseStats.StartingAmmoCount = 1000;

            BaseStats.DashAbility = true;
            BaseStats.StealthAbility = false;
            BaseStats.CarrierAbility = false;
            BaseStats.WeaponSelection = 2;
        }

        public static void ApplyTrophyHull(ref LevelDataStorage.LevelManager.BaseStats BaseStats)
        {
            Debug.Log("test applied trophy hull");

            BaseStats.Health = 5000;
            BaseStats.MaxTurnRate = 80;
            BaseStats.Acceleration = 4120;
            BaseStats.Mass = 3f;
            BaseStats.ScaleFactor = 1f;
            BaseStats.BaseFuel = 100;
            BaseStats.GunOffset = new Vector2(20, 0);
            BaseStats.thrusterLayout = Thrusters.TrophyThrusterSet;
            BaseStats.StartingAmmoCount = 50;

            BaseStats.DashAbility = false;
            BaseStats.StealthAbility = false;
            BaseStats.CarrierAbility = true;
            BaseStats.WeaponSelection = 3;
        }
        public static void ApplyRavenHull(ref LevelDataStorage.LevelManager.BaseStats BaseStats)
        {
            Debug.Log("test applied raven hull");

            BaseStats.Health = 1000;
            BaseStats.MaxTurnRate = 110;
            BaseStats.Acceleration = 5300;
            BaseStats.Mass = 2.75f;
            BaseStats.ScaleFactor = 0.7f;
            BaseStats.BaseFuel = 60;
            BaseStats.GunOffset = new Vector2(10, 1);
            BaseStats.thrusterLayout = Thrusters.RavenThrusterSet;
            BaseStats.StartingAmmoCount = 1400;

            BaseStats.DashAbility = false;
            BaseStats.StealthAbility = true;
            BaseStats.CarrierAbility = false;
            BaseStats.WeaponSelection = 2;
        }
    }
}
public interface ITechNode
{
    string SysName { get; }
    string[] DependencyNodes { get; }
    bool IsNodePurchased { get; set; }
    string DisplayTitle { get; }
    string DisplayText { get; }
    string NodeType { get; }
}