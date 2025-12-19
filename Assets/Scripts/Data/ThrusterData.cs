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
        public float baseScale;

        public Thruster(Vector2 pos, float zRot, float scale)
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
        public ThrusterSet(List<Vector2> Positions, List<float> zRotations, List<float> scales)
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

    public static void ApplyThrusterSet(GameObject parentObj, ThrusterSet th)
    {
        for (int i = 0; i < th.thrusters.Count; i++)
        {
            GameObject obj = ManagerScript.Instance.SpawnPrefab(ManagerScript.Instance.BasicThrusterPrefab,
                                                                th.thrusters[i].position,
                                                                parentObj.transform);
            obj.transform.localScale *= th.thrusters[i].baseScale;
        }
    }

    #region Preset thruster data

    public readonly static ThrusterSet LynchpinThrusterSet =
        new ThrusterSet(
            new List<Vector2> { new(-10, 2.3f), new(-10, -2.3f)},
            new List<float> { 0, 0 },
            new List<float> {1, 1}
        );
    #endregion
}