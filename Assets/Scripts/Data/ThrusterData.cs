using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public static class Thrusters
{
    public class Thruster
    {
        public Vector2 position;
        public float zRotation;
        public Vector2 baseScale;

        public Thruster(Vector2 pos, float zRot, Vector2 scale)
        {
            position = pos;
            zRotation = zRot;
            baseScale = scale;
        }
    }
    public class ThrusterSet
    {
        public List<Thruster> thrusters;

        /// <summary>
        /// Positions and zRotations must have the same length
        /// as the correspond to eachother
        /// </summary>
        /// <param name="Positions"></param>
        /// <param name="zRotations"></param>
        public ThrusterSet(List<Vector2> Positions, List<float> zRotations, List<Vector2> scales)
        {
            if (Positions.Count != zRotations.Count)
            {
                Debug.Log($"Position list {Positions} is of differing length than zRotations list {zRotations}");
                return;
            }

            thrusters = new List<Thruster>();

            for (int i = 0; i < Positions.Count; i++)
            {
                Thruster thisThruster = new(Positions[i], zRotations[i], scales[i]);
                thrusters.Add(thisThruster);
            }
        }
    }

    public static List<GameObject> ApplyThrusterSet(GameObject parentObj, ThrusterSet th)
    {
        List<GameObject> returnThrusters = new();

        for (int i = 0; i < th.thrusters.Count; i++)
        {
            GameObject obj = ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicThrusterPrefab,
                                                                th.thrusters[i].position,
                                                                parentObj.transform);
            obj.transform.localScale *= (Vector2) th.thrusters[i].baseScale;
            returnThrusters.Add(obj);
            obj.transform.localEulerAngles = new Vector3(0, 0, th.thrusters[i].zRotation);
        }

        return returnThrusters;
    }

    #region Preset thruster data

    public readonly static ThrusterSet LynchpinThrusterSet =
        new ThrusterSet(
            new List<Vector2> { new(-8.4f, 2.3f), new(-8.4f, -2.3f)},
            new List<float> { 0, 0 },
            new List<Vector2> {new(0.28f, 0.28f) , new(0.28f, 0.28f)}
        );

    public readonly static ThrusterSet SwallowThrusterSet =
        new ThrusterSet(
            new List<Vector2> { new(-7f, 0), new(-4.47f, -2.75f), new(-4.47f, 2.75f) },
            new List<float> { 0, 42.5f, -42.5f },
            new List<Vector2> { new(0.28f, 0.28f), new(0.15f, 0.15f), new(0.15f, 0.15f) }
            );
    public readonly static ThrusterSet TrophyThrusterSet =
        new ThrusterSet(
            new List<Vector2> { new(-11.79f, -10.36f), new(-11.79f, 10.36f) },
            new List<float> { 0, 0 },
            new List<Vector2> { new(0.7f, 0.7f), new(0.7f, 0.7f) }
            );
    public readonly static ThrusterSet ScorpionThrusterSet =
        new(
            new List<Vector2> { new(-12.5f, 0), new(-10.33f, -8.39f), new(-10.33f, 8.39f) },
            new List<float> { 0, 32.528f, -32.528f },
            new List<Vector2> { new(0.5f, 0.5f) , new(0.4f, 0.4f), new(0.4f, 0.4f) }
        );
    public readonly static ThrusterSet RavenThrusterSet =
        new(
            new List<Vector2> { new(-24.24f, -3.35f), new(-24.24f, 3.35f) },
            new List<float> { 0, 0 },
            new List<Vector2> { new(0.5f, 0.28f), new(0.5f, 0.28f) } // strechted out x
            );
    #endregion
}