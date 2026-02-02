using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    public List<GameObject> Layers;
    private List<float> LayerRotationRates;  // In degrees per second

    private float RotLowBound = -30;
    private float RotHighBound = 30;

    private void Awake()
    {
        GenerateLayerRotationRates();
    }

    private void GenerateLayerRotationRates()
    {
        LayerRotationRates = new List<float>();
        for (int i = 0; i < Layers.Count; i++)
        {
            float rotRate = UnityEngine.Random.Range(RotLowBound, RotHighBound);
            LayerRotationRates.Add(rotRate);
        }
    }

    protected void Update()
    {
        RotateLayersByAngle();
    }

    private void RotateLayersByAngle()
    {
        for (int i = 0; i < Layers.Count;i++)
        {
            Transform t = Layers[i].transform;
            t.Rotate(new Vector3(0, 0, LayerRotationRates[i] * Time.deltaTime));
        }
    }
}
