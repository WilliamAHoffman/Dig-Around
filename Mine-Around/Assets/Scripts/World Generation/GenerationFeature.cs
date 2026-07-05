using UnityEngine;

public abstract class GenerationFeature : WorldGenerationRule
{
    public abstract GenerationResult Apply(
        Vector2Int location,
        float strength,
        GenerationResult result
    );
}