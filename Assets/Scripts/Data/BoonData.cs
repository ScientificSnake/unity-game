using System;
using System.Collections.Generic;
using UnityEngine;
using static BoonData;
public static class BoonData
{

    [CreateAssetMenu(fileName = "NewBoon", menuName = "GameData/Boon")]
    public class BoonBuff : ScriptableObject
    {
        public string SysName;
        public string DisplayName;
        public string Description;
        public int MaxTakes;
        public bool InByDefault; // Whether to include in the boon pool by default
        public string[] Conflicts;
        public BoonEffectType BoonEffect;

        public Sprite Icon;

        public bool UnlockedByDefault;

    }

    public enum BoonEffectType
    {
        TitaniumLiner,
        ExpandedFuel,
        ImprovedBoosters,
        ImprovedBallistics,
        HighRoller
    }


    /// <summary>
    /// Used to store references to the actions for boon buffs
    /// </summary>
    private static class BoonActions
    {
        public static void ApplyTitaniumLiner()
        {
            ManagerScript.CurrentLevelManagerInstance.Modifers.HealthAddtMult += 0.05f;
        }

        public static void ApplyExpandedFuelStores()
        {
            ManagerScript.CurrentLevelManagerInstance.Modifers.FuelAddtMult += 0.05f; // All values subject to change and balancing
        }

        public static void ApplyImprovedAuxillaryBoosters()
        {
            ManagerScript.CurrentLevelManagerInstance.Modifers.TurnRateAddtMult += 0.05f;
        }

        public static void ApplyHighRoller()
        {
            /// double or nothing yo
        }

        public static void ApplyImprovedBallistics()
        {
            ManagerScript.CurrentLevelManagerInstance.Modifers.ShotDragAddtMult += 0.05f;
        }
    }

    //private readonly static BoonBuff TitaniumLiner = new("TitaniumLiner", "Titanium Liner", "Reinforce the spacecrafts surface with Titanium liner to increase survivability.",
    //                                            3, true, new string[] { }, BoonActions.ApplyTitaniumLiner, ManagerScript.Instance.TitaniumLinerSvg, true);

    //private readonly static BoonBuff ExpandedFuelStores = new("ExpandedFuelStores", "Expanded Fuel Stores", "Increase fuel stores by minimizing wasted space in the fuselage.",
    //                                                 2, true, new string[] { }, BoonActions.ApplyExpandedFuelStores, (Sprite)Resources.Load("ResourceSprites/ExpandedFuelStoresIcon"), true);

    //private readonly static BoonBuff ImprovedAuxillaryBoosters = new("ImprovedAuxillaryBoosters", "Improved Auxillary Boosters",
    //                                                        "Increase Turnrate of the space craft by increasing the power of maneuvering thrusters",
    //                                                        4, true, new string[] { }, BoonActions.ApplyImprovedAuxillaryBoosters, (Sprite)Resources.Load("ResourceSprites/ImprovedAuxillaryBoosters"),
    //                                                        true);

    //private readonly static BoonBuff ImprovedBallasitics = new("ImprovedBallastics", "Improved Ballistics",
    //                                                           "Improved round ballistics decrease drag and result in higher round velocity", 3, true, new string[] {},BoonActions.ApplyImprovedBallistics, (Sprite) Resources.Load("ResourceSprites/ImprovedBallisticsIcon"), true);

    //private readonly static BoonBuff HighRoller = new("HighRoller", "High Roller",
    //                                                  "50% chance at the end of a level to gain 2x original reward or nothing.", 1, true,
    //                                                  new string[] { }, BoonActions.ApplyHighRoller, (Sprite)Resources.Load("ResourceSprites/HighRollerIcon"), false);



#pragma warning disable IDE0044 // Add readonly modifier
    private static Dictionary<string, bool> UnlockedBoonDictionary = new();
#pragma warning restore IDE0044 // Add readonly modifier

    public static void UnlockBoon(string sysname)
    {
        UnlockedBoonDictionary[sysname] = true;
    }

    public static bool IsBoonUnlocked(BoonBuff boonBuff)
    {
        if (UnlockedBoonDictionary[boonBuff.SysName])
            return true;
        else return false;
    }
    
    public static HashSet<BoonBuff> GetBoonPool(ref LevelDataStorage.LevelManager levelManager)
    {
        HashSet<BoonBuff> resultantPool = new();


        //foreach (BoonBuff boon in GlobalBoonPool)
        //{
        //    if (boon.InByDefault && IsBoonUnlocked(boon))
        //    {
        //        resultantPool.Add(boon);
        //    }
        //}

        return resultantPool;
    }
}

public static class BoonBuffExtensions
{
    public static bool IsOwned(this BoonBuff boon)
    {
        return IsBoonUnlocked(boon);
    }
}
