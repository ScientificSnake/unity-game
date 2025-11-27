using NUnit.Framework;
using NUnit.Framework.Constraints;
    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Transactions;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelDataStorage
{
    public static System.Random random = new();

    public static float DifficultyDeviationTolerance = 0.1f;
    public static int RoundGenMaxTrys = 100;  // The lion does not concern himself with a 100% chance of level generation

    public class LevelManager // This is the actual object that will be created and returned when entering a level
    {

        public int CurrentRound = 0;
        public List<Dictionary<int, List<Action>>> Rounds;
        public float CurrentScalingMult;
        public List<string> HullOptions; // sys names for active hull option
        public List<string> MajorBoonPool; // sys names for active major boons
        public List<string> StatNodes;

        private static List<string> PurchasedTechNodes(Dictionary<string, TechData.TechNode> nodes)
        {
            List<string> PurchasedTechNodesysNames = new();

            foreach (TechData.TechNode node in nodes.Values)
            {
                if (node.IsNodePurchased == true)
                {
                    PurchasedTechNodesysNames.Add(node.SysName);
                }
            }

            return PurchasedTechNodesysNames;
        }

        public static KeyValuePair<Action, int> GetRandomEventFromDict(Dictionary<Action, int> dict)
        {
            List<Action> keys = dict.Keys.ToList();
            int randomIndex = random.Next(keys.Count);

            Action randomKey = keys[randomIndex];

            return new KeyValuePair<Action, int>(randomKey, dict[randomKey]);
        }

        /// <summary>
        /// [DEPRECIATED]
        ///  Sorts the enabled nodes into the current types : HullOption, Global Stat Boost, MajorBoon
        ///  Returns an array of [Hulloption, stat boost, majorboon] boons respectively
        /// </summary>
        private static List<string>[] SortNodeTypes(Dictionary<string, TechData.TechNode> nodeData, List<string> targetNodes)
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

            foreach (KeyValuePair<Action, int> kvp in kvp_list)
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
                int randomTime = random.Next(31);

                if (timeActionDict.ContainsKey(randomTime))
                {
                    timeActionDict[randomTime].Add(action);
                } else
                {
                    timeActionDict[randomTime] = new List<Action>();
                    timeActionDict[randomTime].Add(action);
                }
            }

            return timeActionDict;
        }
        #endregion

        /// <summary>
        /// Level ManagerConstructor
        /// </summary>
        public LevelManager(LevelData leveldata, Dictionary<string, TechData.TechNode> nodeData)
        {
            List<string> enabledTechNodes = PurchasedTechNodes(nodeData);
            // List<string>[] sortedNodes = SortNodeTypes(nodeData, enabledNodes);

            // constants for indexing sortedNodes DEPRECIATED
            HullOptions = enabledTechNodes;
            //StatNodes = sortedNodes[1]; DEPRECIATED
            //MajorBoonPool = sortedNodes[2]; DEPRECIATED
            CurrentScalingMult = 1;

            //  Round count index starts at 0
            List<Dictionary<int, List<Action>>> RoundList = new();

            for (int round = 0; round < leveldata.RoundCount; round++)
            {
                if (!(leveldata.PresetRounds.ContainsKey(round)))  // IF round is not preset round use this code to generate a round    
                {
                    float targetDifc = leveldata.DifficultyFunc(round);

                    Dictionary<int, List<Action>> roundSched = GenerateRound(targetDifc, leveldata.DifficultyEventDict);
                    RoundList.Add(roundSched);
                }
                else  // Round is a preset
                {
                    RoundList.Add(leveldata.PresetRounds[round]);
                }
            }
            Rounds = RoundList;
        }

        public void DisplayHullSelectionMenu(Transform parentTransform)
        {
            Debug.Log("Displaying hull selection menu at least im trying to ");

            // first determine how many possible hulls there are
            int howManyHullOptions = HullOptions.Count;
            if (howManyHullOptions >= 3)
            {
                // pick 3 from the possible
                // and use the 3 layout

                string[] selectedHullOptions;

                if (howManyHullOptions >= 3)
                {
                    ShufflerNonMono.Shuffle(HullOptions.ToArray());

                    selectedHullOptions = new string[] { HullOptions[0], HullOptions[1], HullOptions[2] };
                }
                else
                {
                    selectedHullOptions = HullOptions.ToArray();
                }

                string firstTargetSysName = selectedHullOptions[0];
                Vector2 firstPositionVector = new(480, 360);
                GameObject firstPrefab = ManagerScript.Instance.SysNameToPrefabObj[firstTargetSysName];
                ManagerScript.Instance.SpawnPrefab(firstPrefab, firstPositionVector, parentTransform);

                string secondTargetSysName = selectedHullOptions[1];
                Vector2 secondPositionVector = new(0, 360);
                GameObject secondPrefab = ManagerScript.Instance.SysNameToPrefabObj[secondTargetSysName];
                ManagerScript.Instance.SpawnPrefab(secondPrefab, secondPositionVector, parentTransform);

                string thirdTargetSysName = selectedHullOptions[2];
                Vector2 thirdPositionVector = new(-480, 360);
                GameObject thirdPrefab = ManagerScript.Instance.SysNameToPrefabObj[thirdTargetSysName];
                ManagerScript.Instance.SpawnPrefab(thirdPrefab, thirdPositionVector, parentTransform);
            }
            else if (howManyHullOptions == 2) // layout for 2
            {
                string firstTargetSysName = HullOptions[0];
                Vector2 firstPositionvector = new Vector2(-320, 360);
                GameObject firstPrefab = ManagerScript.Instance.SysNameToPrefabObj[firstTargetSysName];
                ManagerScript.Instance.SpawnPrefab(firstPrefab, firstPositionvector, parentTransform);

                string secondTargetSysName = HullOptions[1];
                Vector2 secondPositionVector = new Vector2(320, 360);
                GameObject secondPrefab = ManagerScript.Instance.SysNameToPrefabObj[secondTargetSysName];
                ManagerScript.Instance.SpawnPrefab(secondPrefab, secondPositionVector, parentTransform);

            }
            else // layout for 1
            {
                // would iterate of over each but there is only 1

                string targetSysName = HullOptions[0];

                Vector2 positionVector = new(0, 360);

                if (ManagerScript.Instance == null)
                {
                    Debug.LogError("ERROR: ManagerScript.Instance is null!");
                    return;
                }

                if (!ManagerScript.Instance.SysNameToPrefabObj.ContainsKey(targetSysName))
                {
                    Debug.LogError($"ERROR: Dictionary does not contain key: {targetSysName}");
                    return;
                }

                // Check if the value itself is null
                GameObject prefab = ManagerScript.Instance.SysNameToPrefabObj[targetSysName];
                if (prefab == null)
                {
                    Debug.LogError($"ERROR: Prefab assigned to key {targetSysName} in dictionary is null!");
                    return;
                }

                ManagerScript.Instance.SpawnPrefab(prefab, positionVector, parentTransform);
            }

            Vector2 carrierPosition = new(0, -220);

            GameObject CarrierObject = ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.CarrierSpritePrefab, carrierPosition, parentTransform);
            Vector3 carrierscale = new(14, 14, 1);
            CarrierObject.transform.localScale = carrierscale;
        }

        public string selectedHull;

        public Dictionary<string, float> LiveStats;

        public Dictionary<string, float> BaseStats = new(); // Will be populated by the hull stats

        public void FinishHullSelection()
        {
            try
            {
                string targetSysName = selectedHull;

                Action<Dictionary<string, float>> targetMutationFunc = TechData.HullOptionsDataDict[targetSysName].MutationFunc;

                // populate BaseStats Dictionary 
                targetMutationFunc(BaseStats);

                SceneManager.LoadScene("Arena");
            }
            catch
            {
                Debug.Log("you probably need to actually click a hull option dumbass");
            }


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
