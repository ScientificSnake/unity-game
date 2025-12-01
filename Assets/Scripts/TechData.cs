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
                "The basic hull type for your spacecraft - John of all Trades \n\nSTATS:\nHealth: 100\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: None\n\nOne could call this the bricks of the PLF Navy Carrier Regiments. The PLF Navy uses this for all kinds of missions, such as in-atmosphere strikes or basic fleet defense.",
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
                "A faster more agile hull with lower base health and more (instability)\n\nSTATS:\nHealth: 85\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: None\n\nIf the Lynchpin is the bricks, then the Swallow is the mortar. The PLF Navy uses this as a basic interdictor fighter, and occasionally they strap fuel pods onto it and use it for escort missions.",
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
                "An experimental hull with a the new XD-130 drive system, allowing it to dash short distances.\n\nSTATS:\nHealth: 75\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Dash\n\nThis one was a spitball project made by the higher ups after Wraith Industries came up with that dash drive system. Very few prototypes were made because of the price, but it's capable enough that it's very rarely used as a defense fighter. However, to be able to house this equipment and keep its maneuverability it is very fragile, so it's only put into the hands of vetaran pilots.",
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
                "A cargo hull with the ability to carry 2 light fighter drones, due to an improved control computer and added space.\n\nSTATS:\nHealth: 110\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Drone Carrying\n\nAfter the [INSERT CIVIL WAR], the higher ups wanted a way to muster more force to cover more area. After the same competition that resulted in the dash drive concept, Pilen Technologies came up with the Enhanced Capability Trophy Upgrade, or ECTU. This allowed the Trophy to carry 2 light fighter drones, give it the ability to act as an extension of carrier wings.",
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
                "A fighter equipped with a cloaking pod that allows it to deceive the enemy for a limited period of time.\n\nSTATS:\nHealth: 100\nAcceleration Rate : m/s/s\nTurn Rate: degrees/s\nSpecial Ability: Cloaking\n\nSome of the scientists at Wraith Industries came up with this cloaking tech after putting a part from their dash drive in the microwave \"for science\". At the same time they were prototyping a multirole capable of operating both in and out of the atmosphere with minimal changes, so the techs decided to merge the tech and make the Raven. The general public, me included, thought that it would be too expensive to go into full production, but the higher ups saw the potential in it and gave Wraith a contract to produce it at the same scale as the Lynchpin.",
                "SFS-362Raven",
                new string[] {"TrophyHullNode", "ScorpionHullNode"} // positional arg tysh
            )
        }
    };
    #endregion
    #region Node functions for mutation of base stats
    [System.Serializable]
    private static class HullStatsMutationFunctions//RULES TO MAKE IT FEEL GOOD- REDUCE THRUST=REDUCE TURN RATE ADD THRUST=ADD TURN RATE
    {
        public static void ApplyBasicHull(object BaseStatsObj)  // Populating the dictionary of base stats
        {
            Debug.Log("Test applied basic hull");

            if (BaseStatsObj is LevelDataStorage.LevelManager.BaseStats BaseStats)  // OH YEAH REFERENCE MUTATION
            {
                BaseStats.Health = 100; // yeah no we ain't doin no fucking module health
                BaseStats.MaxTurnRate = 107; // degress per second 
                BaseStats.Acceleration = 5000;  // SUBJECT TO FURTHER CHANGE neutons of thrust
                BaseStats.Mass = 2.5f; //mass in kg f-15 weighs 20 tons gross weight NVM THATS A LIE I HAVE NO CLUE -might be multiplicative

                BaseStats.ScaleFactor = 1.75f; //hull size scale factor
                BaseStats.GunOffset = new Vector2(20, 0); //gun offset from center of hull sprite

                BaseStats.BaseFuel = 60; // Fuel in full thrust seconds

                //Special abilities
                BaseStats.DashAbility = false;
                BaseStats.StealthAbility = false;
                BaseStats.CarrierAbility = false;
                BaseStats.WeaponSelection = 1; //adds which weapon dependant on which number is in here(corresponds with its weapon in the list)'
                BaseStats.SelectedWeapon = Sebastian.WeaponryData.WeaponDict[1];  // possible moving to this method instead
            }

        }

        public static void ApplySecondHull(object BaseStatsObj)
        {
            Debug.Log("test applied second hull");
            if (BaseStatsObj is LevelDataStorage.LevelManager.BaseStats BaseStats)
            {
                // print("detected dict object");
                BaseStats.Health = 85; // yeah no we ain't doin no fucking module health
                BaseStats.MaxTurnRate = 112; // degress per second 
                BaseStats.Acceleration = 5000;  // SUBJECT TO FURTHER CHANGE meters per second^2
                BaseStats.Mass = 1.75f;

                BaseStats.ScaleFactor = 1.9f; //hull size scale factor
                BaseStats.BaseFuel = 50;
                BaseStats.GunOffset = new Vector2(20, 0);

                //Special abilities
                BaseStats.DashAbility = false;
                BaseStats.StealthAbility = false;
                BaseStats.CarrierAbility = false;
                BaseStats.WeaponSelection  = 1; //adds which weapon dependant on which number is in here(corresponds with its weapon in the list)
            }
            else { print("argument not dictionary<string, int>"); }
        }

        public static void ApplyScorpionHull(object BaseStatsObj)
        {
            Debug.Log("test applied scorpion hull");
            if (BaseStatsObj is LevelDataStorage.LevelManager.BaseStats BaseStats)
            {
                print("detected dict objec");
                BaseStats.Health = 75; // yeah no we ain't doin no fucking module health
                BaseStats.MaxTurnRate = 130; // degress per second 
                BaseStats.Acceleration = 6000f;  // SUBJECT TO FURTHER CHANGE meters per second^2
                BaseStats.ReverseAcceleration = 10; //^ but in reverse (reverse thrust)
                BaseStats.Mass = 1.5f;
                BaseStats.ScaleFactor = 1.25f; //hull size scale factor
                BaseStats.BaseFuel = 50;
                BaseStats.GunOffset = new Vector2(20, 0);

                //Special abilities
                BaseStats.DashAbility = true;
                BaseStats.StealthAbility = false;
                BaseStats.CarrierAbility = false;
                BaseStats.WeaponSelection = 2; //adds which weapon dependant on which number is in here(corresponds with its weapon in the list)
            }
        }
        public static void ApplyTrophyHull(object BaseStatsObj)
        {
            Debug.Log("test applied trophy hull");

            if (BaseStatsObj is LevelDataStorage.LevelManager.BaseStats BaseStats)
            {
                print("detected dict object");
                BaseStats.Health = 110; // yeah no we ain't doin no fucking module health
                BaseStats.MaxTurnRate = 80; // degress per second 
                BaseStats.Acceleration = 4000;  // SUBJECT TO FURTHER CHANGE meters per second^2
                BaseStats.Mass = 3f;
                BaseStats.ScaleFactor = 1f; //hull size scale factor
                BaseStats.BaseFuel = 100;
                BaseStats.GunOffset = new Vector2(45, 0);  // bro is very large, rocket must be far infortn of him

                //Special abilities
                BaseStats.DashAbility = false;
                BaseStats.StealthAbility = false;
                BaseStats.CarrierAbility = true;
                BaseStats.WeaponSelection = 3; //adds which weapon dependant on which number is in here(corresponds with its weapon in the list)
            }
        }
        public static void ApplyRavenHull(object BaseStatsObj)
        {
            Debug.Log("test applied raven hull");
            if (BaseStatsObj is LevelDataStorage.LevelManager.BaseStats BaseStats)
            {
                print("detected dict object");
                BaseStats.Health = 100; // yeah no we ain't doin no fucking module health
                BaseStats.MaxTurnRate = 110; // degress per second 
                BaseStats.Acceleration = 5500;  // SUBJECT TO FURTHER CHANGE meters per second^2
                BaseStats.Mass = 2.75f;

                BaseStats.ScaleFactor = 0.7f; //hull size scale factor
                BaseStats.BaseFuel = 60;
                BaseStats.GunOffset = new Vector2(20, 0);

                //Special abilities
                BaseStats.DashAbility = false;
                BaseStats.StealthAbility = true;
                BaseStats.CarrierAbility = false;
                BaseStats.WeaponSelection = 2; //adds which weapon dependant on which number is in here(corresponds with its weapon in the list)
            }

        }
        #endregion
    }
}
