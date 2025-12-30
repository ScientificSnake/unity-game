using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using NUnit.Framework.Interfaces;
public class LevelDataStorage
{
    public const int LatestSpawnSecond = 15;

    public static System.Random random = new();

    public static float DifficultyDeviationTolerance = 0.1f;
    public static int RoundGenMaxTrys = 100;  // The lion does not concern himself with a 100% chance of level generation

    public class LevelManager // This is the actual object that will be created and returned when entering a level
    {
        public LevelData RootLevelData;
        public int CurrentRound = 0;
        public List<Dictionary<int, List<Action>>> Rounds;
        public float CurrentScalingMult;
        public List<string> HullOptions; // sys names for active hull option
        public List<string> MajorBoonPool; // sys names for active major boons
        public List<string> StatNodes;

        private static List<string> PurchasedTechNodes(Dictionary<string, TechData.HullNode> nodes)
        {
            List<string> PurchasedTechNodesysNames = new();

            foreach (TechData.HullNode node in nodes.Values)
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

        #region Helper methods for endless level generation
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
                int randomTime = random.Next(LatestSpawnSecond);

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

        public LevelManager(LevelData leveldata, Dictionary<string, TechData.HullNode> nodeData)
        {
            RootLevelData = leveldata;

            List<string> enabledTechNodes = PurchasedTechNodes(nodeData);
            // List<string>[] sortedNodes = SortNodeTypes(nodeData, enabledNodes);

            HullOptions = enabledTechNodes;
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
                    string[] shuffledOptions = HullOptions.ToArray();
                    ShufflerNonMono.Shuffle(shuffledOptions);
                    Debug.Log("3+ hull options");
                    selectedHullOptions = new string[] { shuffledOptions[0], shuffledOptions[1], shuffledOptions[2] };
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
                Vector2 firstPositionvector = new(-320, 360);
                GameObject firstPrefab = ManagerScript.Instance.SysNameToPrefabObj[firstTargetSysName];
                ManagerScript.Instance.SpawnPrefab(firstPrefab, firstPositionvector, parentTransform);

                string secondTargetSysName = HullOptions[1];
                Vector2 secondPositionVector = new(320, 360);
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

        public struct BaseStats
        {
            public float Health;
            public float MaxTurnRate;
            public float Acceleration;
            public float ReverseAcceleration;
            public float Mass;
            public float ScaleFactor;
            public float BaseFuel;
            public float ShotDrag;

            public Thrusters.ThrusterSet thrusterLayout;

            public Vector2 GunOffset;

            public bool DashAbility;
            public bool StealthAbility;
            public bool CarrierAbility;
            public int WeaponSelection;
            public Sebastian.WeaponryData.Weapon SelectedWeapon;
        }

        public BaseStats RootStats = new(); // Will be populated by the hull stats

        public void InstantiatePlayerObject()
        {
            GameObject PlayerPrefabObj = ManagerScript.Instance.PlayerHullObjPrefab;

            Vector2 spawnPos = new(0, 0); // pull from level data maybe ?

            ManagerScript.Instance.SpawnOrphan(PlayerPrefabObj, spawnPos);
        }

        public int RemainingEnemies;
        public bool LastEnemySpawned;
        public int PollTotalEnemies()
        {
            int totalEnemies = 0;

            foreach (string Tag in RootLevelData.PossibleEnemyTags)
            {
                //Debug.Log($"Polling tag {Tag}");
                totalEnemies += ManagerScript.Instance.HowManyObjectsWithTag(Tag);
            }
            //Debug.Log($"{totalEnemies} enemies");
            return totalEnemies;
        }

        public IEnumerator PeriodicallyCheckForEndOfRound(float Period)
        {
            bool enemiesRemain = true;
            while (enemiesRemain)
            {
                yield return new WaitForSeconds(Period);
                if (PollTotalEnemies() == 0 && LastEnemySpawned)
                {
                    enemiesRemain = false;
                    EndRoundRoutine();
                }
                //Debug.Log($"Checking for end of round. Enemies remaining: {PollTotalEnemies()} | Last Enemy Spawned: {LastEnemySpawned}");
            }
        }

        public float RoundEndFadeInTime = 2;

        public void EndRoundRoutine()
        {
            CurrentRound++;
            if (CurrentRound == Rounds.Count)
            {
                Debug.Log($"<color=green> last round detected");
                GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
                // disable player inputs
                PlayerObjectScript PlayerScript = PlayerObj.GetComponent<PlayerObjectScript>();
                PlayerScript.inputManager.Player.Disable();
                PlayerScript.inputManager.CameraControls.Disable();
                Camera MainCam = Camera.main;

                CameraFollowScript CamScript = MainCam.GetComponent<CameraFollowScript>();

                CamScript.inputManager.CameraControls.Disable();

                PlayerScript.throttle = 0;

                GameObject.Destroy(PlayerObj);

                GameObject Layout = GameObject.FindGameObjectWithTag("MapLayoutTag");
                GameObject.Destroy(Layout);

                GameObject[] Artifacts = GameObject.FindGameObjectsWithTag("InstantiatedArtifact");

                foreach (GameObject Artifact in Artifacts)
                {
                    GameObject.Destroy(Artifact);
                }

                Debug.Log("Last round reached at round " + CurrentRound);

                ManagerScript.Instance.StartCoroutine(ManagerScript.Instance.LoadSceneRoutine("WinScreen"));
            }
            else
            {
                Debug.Log("-------------------------------------- End of round test");
                GameObject RoundOverScreen = GameObject.FindGameObjectWithTag("RoundOverScreen");
                CanvasGroup RoundOverCG = RoundOverScreen.GetComponent<CanvasGroup>();
                ManagerScript.Instance.StartCoroutine(ManagerScript.Instance.FadeInCanvasImage(RoundOverCG, RoundEndFadeInTime));

                GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
                // disable player inputs
                PlayerObjectScript PlayerScript = PlayerObj.GetComponent<PlayerObjectScript>();
                PlayerScript.inputManager.Player.Disable();
                PlayerScript.inputManager.CameraControls.Disable();
                Camera MainCam = Camera.main;

                CameraFollowScript CamScript = MainCam.GetComponent<CameraFollowScript>();

                CamScript.inputManager.CameraControls.Disable();

                PlayerScript.throttle = 0;

                void RunAfterFadeIn()
                {
                    //set velo to 0

                    Rigidbody2D PlayerRb = PlayerObj.GetComponent<Rigidbody2D>();
                    PlayerRb.linearVelocity = Vector2.zero;
                    PlayerRb.angularVelocity = 0;

                    GameObject Layout = GameObject.FindGameObjectWithTag("MapLayoutTag");
                    UnityEngine.Object.Destroy(Layout);

                    GameObject RoundOverScreen = GameObject.FindGameObjectWithTag("RoundOverScreen");
                    RoundOverScreen.GetComponent<RoundOverImage>().EndOfRoundButtons.SetActive(true);
                    CanvasGroup RoundOverCG = RoundOverScreen.GetComponent<CanvasGroup>();
                    RoundOverCG.interactable = true;
                    RoundOverCG.blocksRaycasts = true;
                }

                ManagerScript.Instance.RunOnDelay(RunAfterFadeIn, RoundEndFadeInTime);

                // we can just get rid of all bullets now, its spreads the frame hitch anyways so

                GameObject[] weaponArtifacts = GameObject.FindGameObjectsWithTag("InstantiatedArtifact");

                foreach (GameObject weaponArtifact in weaponArtifacts)
                {
                    UnityEngine.Object.Destroy(weaponArtifact);
                }
            }
        }

        public class PlayerStatModifers
        {
            public float HealthAddtMult;
            public float FuelAddtMult;
            public float TurnRateAddtMult;
            public float ShotDragAddtMult;

            public BaseStats MutateBaseStats(BaseStats stats)
            {
                BaseStats result = stats;
                result.Health = stats.Health * HealthAddtMult;
                result.BaseFuel = stats.BaseFuel * FuelAddtMult;
                result.MaxTurnRate = stats.MaxTurnRate * TurnRateAddtMult;
                result.ShotDrag = stats.MaxTurnRate * ShotDragAddtMult;

                return result;
            }

            public PlayerStatModifers()
            {
                HealthAddtMult = 1;
                FuelAddtMult = 1;
                TurnRateAddtMult = 1;
                ShotDragAddtMult = 1;
            }
        }

        public PlayerStatModifers Modifers;

        // Getoverhere start
        public void StartRoundRoutine() 
        {   
            Time.timeScale = 1.0f;

            GameObject Player;
            PlayerObjectScript PlayerScript;
            if (CurrentRound == 0)
            {
                Modifers = new();

                ManagerScript.CurrentLevelManagerInstance.InstantiatePlayerObject();
                Player = GameObject.FindWithTag("Player");
                SpriteRenderer spriteRenderer = Player.GetComponent<SpriteRenderer>();

                spriteRenderer.sprite = ManagerScript.Instance.SpriteDict[ManagerScript.CurrentLevelManagerInstance.selectedHull];
                PlayerScript = Player.GetComponent<PlayerObjectScript>();
            } else
            {
                Player = GameObject.FindWithTag("Player");
                PlayerScript = Player.GetComponent<PlayerObjectScript>();
                PlayerScript.inputManager.Enable();
                PlayerScript.inputManager.Player.Enable();
                PlayerScript.mainCamera.GetComponent<CameraFollowScript>().inputManager.CameraControls.Enable();
            }

            PlayerScript.RoundStats = Modifers.MutateBaseStats(RootStats);
            Debug.Log($"<color=yellow> Player weapon selection is {RootStats.WeaponSelection}, turns into {PlayerScript.RoundStats.WeaponSelection}");

            PlayerScript.ResetRoundStats();
            // hide the round over screen
            Debug.Log("Start round routine triggered" + $" | Starting round {CurrentRound}");
            try
            {
                //GameObject RoundOverScreen = GameObject.FindGameObjectWithTag("RoundOverScreen");

                GameObject RoundOverScreen = GameObject.FindWithTag("RoundOverScreen");

                CanvasGroup canvasGroup = RoundOverScreen.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            catch(Exception e)
            {
                Debug.LogError("Failed setting round overscreen to transparent " + e);
                return;
            }

            Debug.Log($"There are {Rounds.Count} Rounds");

            Dictionary<int, List<Action>> RoundDict = Rounds[CurrentRound];

            // start co routine "timers" on the events

            //Debug.Log($"Current Round is {RoundDict.ToString()}");

            if (RootLevelData.IsEndless is false)
            {
                // first slime the previous layout
                GameObject PreviousLayoutObj = GameObject.FindWithTag("MapLayoutTag");
                ManagerScript.Destroy(PreviousLayoutObj);

                ManagerScript.Instantiate(RootLevelData.LayoutPrefab[CurrentRound]);
                // reset player position and velocity and health and fuel
            }

            int localLatestSpawnTime = RoundDict.Keys.Max();
            ManagerScript.Instance.StartCoroutine(ManagerScript.Instance.StartLastEnemySpawnTimer(localLatestSpawnTime));
            ManagerScript.Instance.StartCoroutine(PeriodicallyCheckForEndOfRound(0.5f));  // wanted in 3 countries for this move :(
            Debug.Log("started periodic enemy checking");
        }

        public void GameOver()
        { 
            Time.timeScale = 1.0f;
            GameObject LevelLayout = GameObject.FindWithTag("MapLayoutTag");
            UnityEngine.Object.Destroy(LevelLayout);

            GameObject[] weaponArtifacts = GameObject.FindGameObjectsWithTag("InstantiatedArtifact");

            foreach (GameObject weaponArtifact in weaponArtifacts)
            {
                UnityEngine.Object.Destroy(weaponArtifact);
            }

            GameObject Player = GameObject.FindWithTag("Player");

            PlayerObjectScript playerScript = Player.GetComponent<PlayerObjectScript>();

            playerScript.mainCamera.GetComponent<CameraFollowScript>().inputManager.CameraControls.Disable();
            playerScript.inputManager.Player.Disable();

            UnityEngine.Object.Destroy(Player);

            ManagerScript.Instance.StopCoroutine("PeriodicallyCheckForEndOfRound");
            ManagerScript.Instance.StopCoroutine("StartLastEnemySpawnTimer");
            ManagerScript.Instance.StartCoroutine(ManagerScript.Instance.LoadSceneRoutine("GameOverScreen"));
            ManagerScript.CurrentLevelManagerInstance = null;
        }
    }
    public class LevelData
    {
        public bool IsEndless { get; } = false;

        public int RoundCount { get; }  // Number of rounds in level
        public Func<int, int> DifficultyFunc { get; }  // Round -> Difficulty for that round
        public Dictionary<Action, int> DifficultyEventDict { get; } // Action to corresponding difficulty
        public Dictionary<int, Dictionary<int, List<Action>>> PresetRounds { get; }  // (which round is preset) -> Dictionary of Time(seconds) -> Actions []

        public GameObject[] LayoutPrefab;
        public string[] PossibleEnemyTags { get; }

        public Vector2 MinXY;
        public Vector2 MaxXY;

        public LevelData(int roundCount, Func<int, int> difficultyFunc, Dictionary<Action, int> difficultyEventDict, Dictionary<int, Dictionary<int, List<Action>>> presetRounds,
                         string[] possibleEnemyTags, GameObject[] layoutPrefab, bool Endless, Vector2 minXY, Vector2 maxXY)
        {
            RoundCount = roundCount;
            DifficultyFunc = difficultyFunc;
            DifficultyEventDict = difficultyEventDict;
            PresetRounds = presetRounds;
            PossibleEnemyTags = possibleEnemyTags;

            LayoutPrefab = layoutPrefab;
            MinXY = minXY;
            MaxXY = maxXY;
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
