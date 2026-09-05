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
        [SerializeField, Range(0, 1f)]
        private float min = 0;

        [SerializeField, Range(0, 1f)]
        private float max = 1f;

        [SerializeField]
        private FactalWorldLayer layer;

        public float Min => Mathf.Min(min, max);
        public float Max => Mathf.Max(min, max);
        public FactalWorldLayer Layer => layer;

        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
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

        GenerateSubLayers(sample, ref context);
    }

    private void GenerateSubLayers(float sample, ref GenerationContext generationContext)
    {

        for (int i = 0; i < subLayers.Count; i++)
        {
            SubLayerRange entry = subLayers[i];

            if (entry == null || entry.Layer == null)
                continue;

            if (!entry.Contains(sample))
                continue;

            // Strictly greater preserves Inspector order when two matches tie.
            entry.Layer.Generate(ref generationContext);
        }
    }
}
