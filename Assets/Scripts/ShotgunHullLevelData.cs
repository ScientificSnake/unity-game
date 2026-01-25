using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelDataStorage;
public class ShotgunHullLevelData
{
    private static int DifficultyFunc(int Round)
    {
        if (Round == 0) { return 10; }
        else if (Round == 1) { return 15; }
        else { return 10; }
    }

    public static LevelData Main = new(
    1,
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
            1,
            new Dictionary<int, List<Action>>
            {
                {
                    1,
                    new List<Action>
                    {
                        EventLib.TestEvent
                    }
                }
            }
        }
    },
    new string[] { "EnemyTag" },
    new GameObject[] {ManagerScript.Instance.ShotgunHell},
    Endless: false,
    new Vector2(-2000, -2000),
    new Vector2(2000, 2000),
    "Shotgun Hell"
    );
}
