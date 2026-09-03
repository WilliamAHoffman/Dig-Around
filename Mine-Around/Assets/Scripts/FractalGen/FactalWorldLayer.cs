using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FactalWorldLayer",
    menuName = "FractalGen/WorldLayer")]
public class FactalWorldLayer : ScriptableObject
{
    [Serializable]
    public class SubLayerRange
    {
        [SerializeField, Range(-1f, 1f)]
        private float min = -1f;

        [SerializeField, Range(-1f, 1f)]
        private float max = 1f;

        [SerializeField]
        private FactalWorldLayer layer;

        public float Min => Mathf.Min(min, max);
        public float Max => Mathf.Max(min, max);
        public float Center => (Min + Max) * 0.5f;
        public FactalWorldLayer Layer => layer;

        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }

        public float DistanceFromCenter(float value)
        {
            return Mathf.Abs(value - Center);
        }
    }

    [Header("Layer Selection")]
    [SerializeField] private NoiseSettings noise;
    [SerializeField] private List<SubLayerRange> subLayers = new();

    [Header("Decoration")]
    [SerializeField] private FractalDecorator baseDecor;

    public void Generate(ref GenerationContext context)
    {
        if (baseDecor != null)
            baseDecor.Apply(ref context);

        if (noise == null || subLayers == null || subLayers.Count == 0)
            return;

        float sample = noise.Sample(
            context.Position);

        FactalWorldLayer selected = SelectSubLayer(sample);

        if (selected != null)
            selected.Generate(ref context);
    }

    private FactalWorldLayer SelectSubLayer(float sample)
    {
        FactalWorldLayer selected = null;
        float greatestDistanceFromCenter = float.NegativeInfinity;

        for (int i = 0; i < subLayers.Count; i++)
        {
            SubLayerRange entry = subLayers[i];

            if (entry == null || entry.Layer == null)
                continue;

            if (!entry.Contains(sample))
                continue;

            float distanceFromCenter = entry.DistanceFromCenter(sample);

            // Strictly greater preserves Inspector order when two matches tie.
            if (distanceFromCenter > greatestDistanceFromCenter)
            {
                greatestDistanceFromCenter = distanceFromCenter;
                selected = entry.Layer;
            }
        }

        return selected;
    }
}
