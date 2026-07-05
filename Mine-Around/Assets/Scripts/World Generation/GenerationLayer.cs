using System.Collections.Generic;
using UnityEngine;

public enum GenerationLayerMode
{
    PickBest,
    ApplyAll
}

[CreateAssetMenu(fileName = "GenerationLayer", menuName = "World Generation/Generation Layer")]
public class GenerationLayer : ScriptableObject
{
    [Header("Features")]
    public List<GenerationFeature> features;

    [Header("Mode")]
    public GenerationLayerMode mode = GenerationLayerMode.PickBest;

    public GenerationResult Generate(Vector2Int location, WorldSample worldSample, GenerationResult result)
    {
        if (features == null || features.Count == 0)
            return result;

        switch (mode)
        {
            case GenerationLayerMode.PickBest:
                return GenerateBest(location, worldSample, result);

            case GenerationLayerMode.ApplyAll:
                return GenerateAll(location, worldSample, result);

            default:
                return result;
        }
    }

    private GenerationResult GenerateBest(Vector2Int location, WorldSample worldSample, GenerationResult result)
    {
        GenerationFeature bestFeature = null;
        float bestScore = float.MinValue;

        foreach (GenerationFeature feature in features)
        {
            if (feature == null)
                continue;

            float score = feature.Similarity(worldSample);

            if (score < feature.minSimilarity)
                continue;

            if (bestFeature == null || score > bestScore)
            {
                bestFeature = feature;
                bestScore = score;
            }
        }

        if (bestFeature == null)
            return result;

        return bestFeature.Apply(location, bestScore, result);
    }

    private GenerationResult GenerateAll(Vector2Int location, WorldSample worldSample, GenerationResult result)
    {
        foreach (GenerationFeature feature in features)
        {
            if (feature == null)
                continue;

            float score = feature.Similarity(worldSample);

            if (score < feature.minSimilarity)
                continue;

            result = feature.Apply(location, score, result);
        }

        return result;
    }
}