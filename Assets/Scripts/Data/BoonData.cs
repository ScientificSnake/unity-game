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

        public bool Unlocked;


        public BoonBuff (string sysName, string displayName, string description, int maxTakes, bool inByDefault, string[] conflicts, Action action, Sprite icon, bool unlocked)
        {
            SysName = sysName;
            DisplayName = displayName;
            Description = description;
            MaxTakes = maxTakes;
            InByDefault = inByDefault;
            Conflicts = conflicts;
            AppliedAction = action;
            Icon = icon;

            Unlocked = unlocked;
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
    }

    private static BoonBuff TitaniumLiner = new("TitaniumLiner", "Titanium Liner", "Reinforce the spacecrafts surface with Titanium liner to increase survivability.",
                                                3, true, new string[] { }, BoonActions.ApplyTitaniumLiner, (Sprite) Resources.Load("ResourceSprites/TitaniumLinerIcon"), true);

    private static BoonBuff ExpandedFuelStores = new("ExpandedFuelStores", "Expanded Fuel Stores", "Increase fuel stores by minimizing wasted space in the fuselage.",
                                                     2, true, new string[] { }, BoonActions.ApplyExpandedFuelStores, (Sprite)Resources.Load("ResourceSprites/ExpandedFuelStoresIcon"), true);

    private static BoonBuff[] GlobalBoonPool = new BoonBuff[]
    {
        TitaniumLiner,
        ExpandedFuelStores
    };

    public static HashSet<BoonBuff> GetBoonPool(ref LevelDataStorage.LevelManager levelManager)
    {
        HashSet<BoonBuff> resultantPool = new();


        foreach (BoonBuff boon in GlobalBoonPool)
        {
            if (boon.InByDefault && boon.Unlocked)
            {
                resultantPool.Add(boon);
            }
        }

        return resultantPool;
    }



}
