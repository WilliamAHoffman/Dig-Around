using UnityEngine;

[CreateAssetMenu(fileName = "MountainFeature", menuName = "World Generation/Features/Mountain Feature")]
public class MountainFeature : GenerationFeature
{
    [Header("Noise")]
    public NoiseSettings densityNoise;

    [Header("Mountain Settings")]
    [Range(0f, 1f)] public float mountainCutoff = 0.6f;

    [Tooltip("Extra control over how strong this mountain feature is.")]
    [Range(0f, 2f)] public float mountainStrength = 1f;

    [Header("Rule")]
    [SerializeField] private GenerationRule rule;

    public override GenerationResult Apply(Vector2Int location, float strength, GenerationResult result)
    {
        if (densityNoise == null || rule == null)
            return result;

        float density = densityNoise.Sample(location.x, location.y);

        float mountainAmount = density * strength * mountainStrength;

        if (mountainAmount < mountainCutoff)
            return result;

        return rule.Apply(result);
    }
}