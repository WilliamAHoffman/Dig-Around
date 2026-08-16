using System.Collections.Generic;
using UnityEngine;

public enum GenerationLayerMode
{
    PickBest,
    ApplyAll
}

[CreateAssetMenu(
    fileName = "GenerationLayer",
    menuName = "World Generation/Generation Layer"
)]
public class GenerationLayer : ScriptableObject
{
    [Header("Features")]
    [SerializeField]
    private List<GenerationFeature> features = new();

    [Header("Mode")]
    [SerializeField]
    private GenerationLayerMode mode =
        GenerationLayerMode.PickBest;

    public MapCell Generate(
        Vector2Int location,
        WorldSample worldSample,
        MapCell result)
    {
        if (features == null ||
            features.Count == 0)
        {
            return result;
        }

        return mode switch
        {
            GenerationLayerMode.PickBest =>
                GenerateBest(
                    location,
                    worldSample,
                    result
                ),

            GenerationLayerMode.ApplyAll =>
                GenerateAll(
                    location,
                    worldSample,
                    result
                ),

            _ => result
        };
    }

    #region Pick Best

    private MapCell GenerateBest(
        Vector2Int location,
        WorldSample worldSample,
        MapCell result)
    {
        GenerationFeature bestFeature = null;
        float bestScore = float.MinValue;

        foreach (GenerationFeature feature in features)
        {
            if (feature == null)
            {
                continue;
            }

            float score =
                feature.Similarity(worldSample);

            if (score < feature.MinSimilarity)
            {
                continue;
            }

            if (bestFeature != null &&
                score <= bestScore)
            {
                continue;
            }

            bestFeature = feature;
            bestScore = score;
        }

        if (bestFeature == null)
        {
            return result;
        }

        return bestFeature.Apply(
            location,
            bestScore,
            result
        );
    }

    #endregion

    #region Apply All

    private MapCell GenerateAll(
        Vector2Int location,
        WorldSample worldSample,
        MapCell result)
    {
        foreach (GenerationFeature feature in features)
        {
            if (feature == null)
            {
                continue;
            }

            float score =
                feature.Similarity(worldSample);

            if (score < feature.MinSimilarity)
            {
                continue;
            }

            result = feature.Apply(
                location,
                score,
                result
            );
        }

        return result;
    }

    #endregion
}