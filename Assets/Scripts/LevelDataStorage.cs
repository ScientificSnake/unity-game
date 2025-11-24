using NUnit.Framework;
using NUnit.Framework.Constraints;
    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Transactions;
using UnityEngine;
using static TechData;

public class LevelDataStorage
{


    public static float DifficultyDeviationTolerance = 0.1f;
    public static int RoundGenMaxTrys = 100;

    public class LevelManager // This is the actual object that will be created and returned when entering a level
    {
        public List<Dictionary<int, List<Action>>> Rounds;
        public float CurrentScalingMult;
        public List<string> HullOptions; // sys names for active hull option
        public List<string> MajorBoonPool; // sys names for active major boons
        public List<string> StatNodes;

        private static List<string> PurchasedBoons(Dictionary<string, TechNode> nodes)
        {
            List<string> purchasedBoonSysNames = new();

            foreach (TechNode node in nodes.Values)
            {
                if (node.IsNodePurchased == true)
                {
                    purchasedBoonSysNames.Add(node.SysName);
                }
            }

            return purchasedBoonSysNames;
        }

        public static KeyValuePair<Action, int> GetRandomEventFromDict(Dictionary<Action, int> dict)
        {
            List<Action> keys = dict.Keys.ToList();
            int randomIndex = UnityEngine.Random.Range(0, keys.Count);

            Action randomKey = keys[randomIndex];

            return new KeyValuePair<Action, int>(randomKey, dict[randomKey]);
        }

        /// <summary>
        ///  Sorts the enabled nodes into the current types : HullOption, Global Stat Boost, MajorBoon
        ///  Returns an array of [Hulloption, stat boost, majorboon] boons respectively
        /// </summary>
        private static List<string>[] SortNodeTypes(Dictionary<string, TechNode> nodeData, List<string> targetNodes)
        {
            List<string> statNodes = new();
            List<string> hullOptions = new();
            List<string> majorBoons = new();

            foreach (string sysName in targetNodes)
            {
                string nodeType = nodeData[sysName].NodeType;
                if (nodeType == "HullOption")  // Important use exact node type name
                {
                    hullOptions.Add(sysName);
                }
                else if (nodeType == "MajorBoon")
                {
                    majorBoons.Add(sysName);
                }
                else // Assume global stat now
                {
                    statNodes.Add(sysName);
                }
            }
            List<string>[] sortedNodes = new List<string>[] { hullOptions, statNodes, majorBoons };

            return sortedNodes;
        }

        #region Helper methods for round generation
        private static int SumDifficultys(List<KeyValuePair<Action, int>> kvp_list)
        {
            int totalDifc = 0;

            foreach (KeyValuePair<Action, int> kvp in kvp_list)
            {
                int difc = kvp.Value;
                totalDifc += difc;
            }
            return totalDifc;
        }

        private static List<Action> GetActionsFromAllDifc(List<KeyValuePair<Action, int>> kvp_list)
        {
            List<Action> actionList = new();

            foreach(KeyValuePair<Action,int> kvp in kvp_list)
            {
                actionList.Add(kvp.Key);
            }

            return actionList;
        }

        private static Dictionary<int, List<Action>> GenerateRound(float TargetDifficultySum, Dictionary<Action, int> EventDifficultyDict)
        {
            float lowBound = (TargetDifficultySum * (1 - DifficultyDeviationTolerance));
            float highBound = (TargetDifficultySum * (1 + DifficultyDeviationTolerance));

            List<Action> chosenActionCombination = new();

            for (int i = 0; i < (RoundGenMaxTrys - 1); i++)
            {
                List<KeyValuePair<Action, int>> allActionDifcs = new();
                int currentDifficultySum;
                bool foundActionCombination = false;
                while (true)
                {
                    KeyValuePair<Action, int> actionDifc = GetRandomEventFromDict(EventDifficultyDict);
                    allActionDifcs.Add(actionDifc);
                    currentDifficultySum = SumDifficultys(allActionDifcs);
                    if ((currentDifficultySum >= lowBound) && (currentDifficultySum <= highBound))  // If we find a valid combo get out of the loop
                    {
                        chosenActionCombination = GetActionsFromAllDifc(allActionDifcs);
                        foundActionCombination = true;
                        break;
                    } else if (currentDifficultySum >= highBound)  // If we exceed threshold get out of the while loop
                    {
                        allActionDifcs.Clear();
                        break;
                    }
                }
                if (foundActionCombination) // if we found one break out of the for loop
                {
                    break;
                }
            }
            // now randomly assign times for each action in choseActionCombination

            Dictionary<int, List<Action>> timeActionDict = new();

            foreach (Action action in chosenActionCombination)
            {
                int randomTime = UnityEngine.Random.Range(0, 31);
                timeActionDict[randomTime].Add(action);
            }

            return timeActionDict;
        }
        #endregion

        public LevelManager(LevelData leveldata, Dictionary<string, TechNode> nodeData)
        {
            List<string> enabledNodes = PurchasedBoons(nodeData);
            List<string>[] sortedNodes = SortNodeTypes(nodeData, enabledNodes);

            // constants for indexing sortedNodes
            HullOptions = sortedNodes[0];
            StatNodes = sortedNodes[1];
            MajorBoonPool = sortedNodes[2];
            CurrentScalingMult = 1;

            //  Round count index starts at 0
            List<Dictionary<int, List<Action>>> RoundList = new();

            


            for (int i = 0; i < leveldata.RoundCount; i++)
            {
                float targetDifc = leveldata.DifficultyFunc(i);

                Dictionary<int, List<Action>> round = GenerateRound(targetDifc, leveldata.DifficultyEventDict);
                RoundList.Add(round);
            }
            Rounds = RoundList;

            
        }
    }
    public class LevelData
    {
        public int RoundCount { get; }  // Number of rounds in level
        public Func<int, int> DifficultyFunc { get; }  // Round -> Difficulty for that round
        public Dictionary<Action, int> DifficultyEventDict { get; } // Action to corresponding difficulty
        public Dictionary<int, Dictionary<int, List<Action>>> PresetRounds { get; }  // (which round is preset) -> Dictionary of Time(seconds) -> Actions []

        public LevelData(int roundCount, Func<int, int> difficultyFunc, Dictionary<Action, int> difficultyEventDict, Dictionary<int, Dictionary<int, List<Action>>> presetRounds)
        {
            RoundCount = roundCount;
            DifficultyFunc = difficultyFunc;
            DifficultyEventDict = difficultyEventDict;
            PresetRounds = presetRounds;
        }
    }

    public static class EventLib
    {
        public static void SpawnTutorialDummy()
        {
            Debug.Log("Spawning test dummy");
        }

        public static void TestEvent()
        {
            Debug.Log("test event. maybe a solar flare that damages");
        }
    }

    public static Dictionary<string, LevelData> LevelDataDict = new()
    {
        {
            "Tutorial",
            TuturialLevelData.Main
        }
    };
}
