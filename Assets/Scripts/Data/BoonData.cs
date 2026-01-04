using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static BoonData;
using System.Linq;
using NUnit.Framework;
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

    private static Action[] correspondingActions = 
    {
        BoonActions.ApplyTitaniumLiner,
        BoonActions.ApplyExpandedFuelStores,
        BoonActions.ApplyImprovedAuxillaryBoosters,
        BoonActions.ApplyImprovedBallistics,
        BoonActions.ApplyHighRoller
    };

#pragma warning disable IDE0044 // Add readonly modifier
    private static List<string> UnlockedBoons = new();
#pragma warning restore IDE0044 // Add readonly modifier

    public static void UnlockBoon(string sysname)
    {
        UnlockedBoons.Add(sysname);
        Debug.Log($"<color=orange> Unlocking {sysname}");
    }

    public static bool IsBoonUnlocked(BoonBuff boonBuff)
    {
        if (UnlockedBoons.Contains(boonBuff.SysName))
            return true;
        else return false;
    }
    
    public static HashSet<BoonBuff> GetBoonPool(LevelDataStorage.LevelManager levelManager)
    {
        HashSet<BoonBuff> resultantPool = new();


        foreach (BoonBuff boon in ManagerScript.Instance.MasterBoonList)
        {
            if (boon.InByDefault && IsBoonUnlocked(boon))
            {
                resultantPool.Add(boon);
            }
        }
        return resultantPool;
    }

    public static BoonBuff[] GetXRandomBoons(int xCount, LevelDataStorage.LevelManager levelManager)
    {
        BoonBuff[] applicablePool = GetBoonPool(levelManager).ToArray(); 

        HashSet<int> picks = new();

        xCount = Mathf.Min(xCount, applicablePool.Length);

        while (picks.Count < xCount)
        {
            int randomBoonBuffIndex = UnityEngine.Random.Range(0, applicablePool.Length);
            picks.Add(randomBoonBuffIndex);
        }

        BoonBuff[] pickedBoonBuffs = new BoonBuff[picks.Count];
        int[] picksArray = picks.ToArray();

        for (int i = 0; i < picks.Count; i++)
        {
            pickedBoonBuffs[i] = applicablePool[picksArray[i]];
        }

        return pickedBoonBuffs;
    }

    public static Action GetBoonEffect(BoonBuff boon)
    {
        int boonActionIndex = (int)boon.BoonEffect;

        Action boonAction = correspondingActions[boonActionIndex];

        return boonAction;
    }
}

public static class BoonBuffExtensions
{
    /// <summary>
    /// Alternative to BoonData.IsBoonUnlocked()
    /// </summary>
    /// <param name="boon"></param>
    /// <returns></returns>
    public static bool IsOwned(this BoonBuff boon)
    {
        return IsBoonUnlocked(boon);
    }
}
