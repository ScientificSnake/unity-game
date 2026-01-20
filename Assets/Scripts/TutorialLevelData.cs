using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelDataStorage;
public static class TuturialLevelData
{
    private static int DifficultyFunc(int Round)
    {
        if (Round == 0) { return 10; }
        else if (Round == 1) { return 15; }
        else { return 10; }
    }

    public static LevelData Main = new(
        2,
        DifficultyFunc,  // This is the function for calculating difficulty per round, here it doesn't really do much but in longer rounds it will simplify things.
        new Dictionary<Action, int>  // Events as values and their difficulty as keys
        {
            {
                EventLib.SpawnTutorialDummy,
                5
            },
            {
                EventLib.TestEvent,
                5
            }
        },
        new Dictionary<int, Dictionary<int, List<Action>>> // This is the dict of preset level
        {
            {
                1,  // 1 is the preset level
                new Dictionary<int, List<Action>>
                {
                    {
                        2, // At second 2 do these actions
                        new List <Action>
                        {
                            EventLib.SpawnTutorialDummy,
                            EventLib.TestEvent
                        }
                    }
                }
            },
            {
                2,
                new Dictionary<int, List<Action>>
                {
                    {
                    0,
                        new List<Action>
                        {
                            EventLib.TestEvent
                        }
                    }
                }
            }
        },
        new string[] { "EnemyTag" },
        new GameObject[] { ManagerScript.Instance.TutorialLayoutRound0, ManagerScript.Instance.TutorialLayoutPrefab },
        Endless: false,
        new Vector2(-2000, -2000),
        new Vector2(2000, 2000),
        "Tutorial"
        );
}
