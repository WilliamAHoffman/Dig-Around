using UnityEngine;

public abstract class GenerationFeature : WorldGenerationRule
{
    public abstract MapCell Apply(
        Vector2Int location,
        float strength,
        MapCell result
    );
}