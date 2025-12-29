using System;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class BoonData
{
    public class BoonBuff
    {
        public readonly string SysName;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly int MaxTakes;
        public readonly bool InByDefault; // Whether to include in the boon pool by default
        public readonly string[] Conflicts;
        public readonly Action AppliedAction;

        public readonly Sprite Icon;

        public readonly bool UnlockedByDefault;

        public BoonBuff (string sysName, string displayName, string description, int maxTakes, bool inByDefault, string[] conflicts, Action action, Sprite icon, bool unlockedByDefault)
        {
            SysName = sysName;
            DisplayName = displayName;
            Description = description;
            MaxTakes = maxTakes;
            InByDefault = inByDefault;
            Conflicts = conflicts;
            AppliedAction = action;
            Icon = icon;
            UnlockedByDefault = unlockedByDefault;
        }
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
    }

    private readonly static BoonBuff TitaniumLiner = new("TitaniumLiner", "Titanium Liner", "Reinforce the spacecrafts surface with Titanium liner to increase survivability.",
                                                3, true, new string[] { }, BoonActions.ApplyTitaniumLiner, (Sprite) Resources.Load("ResourceSprites/TitaniumLinerIcon"), true);

    private readonly static BoonBuff ExpandedFuelStores = new("ExpandedFuelStores", "Expanded Fuel Stores", "Increase fuel stores by minimizing wasted space in the fuselage.",
                                                     2, true, new string[] { }, BoonActions.ApplyExpandedFuelStores, (Sprite)Resources.Load("ResourceSprites/ExpandedFuelStoresIcon"), true);

    private readonly static BoonBuff ImprovedAuxillaryBoosters = new("ImprovedAuxillaryBoosters", "Improved Auxillary Boosters",
                                                            "Increase Turnrate of the space craft by increasing the power of maneuvering thrusters",
                                                            4, true, new string[] { }, BoonActions.ApplyImprovedAuxillaryBoosters, (Sprite)Resources.Load("ResourceSprites/ImprovedAuxillaryBoosters"),
                                                            true);

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

    public readonly static BoonBuff[] GlobalBoonPool = new BoonBuff[]
    {
        TitaniumLiner,
        ExpandedFuelStores,
        ImprovedAuxillaryBoosters
    };

    public static HashSet<BoonBuff> GetBoonPool(ref LevelDataStorage.LevelManager levelManager)
    {
        HashSet<BoonBuff> resultantPool = new();


        foreach (BoonBuff boon in GlobalBoonPool)
        {
            if (boon.InByDefault && IsBoonUnlocked(boon))
            {
                resultantPool.Add(boon);
            }
        }

        return resultantPool;
    }



}
