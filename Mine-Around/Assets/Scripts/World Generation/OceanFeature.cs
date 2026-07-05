using UnityEngine;

[CreateAssetMenu(fileName = "OceanFeature", menuName = "World Generation/Features/Ocean Feature")]
public class OceanFeature : GenerationFeature
{
    [Header("Noise")]
    public NoiseSettings densityNoise;

    [Header("Ocean Settings")]
    [Range(0f, 1f)] public float oceanCutoff = 0.6f;

    [Tooltip("Extra control over how strong this ocean feature is.")]
    [Range(0f, 2f)] public float oceanStrength = 1f;

    [Header("Rule")]
    [SerializeField] private GenerationRule rule;

    public override GenerationResult Apply(Vector2Int location, float strength, GenerationResult result)
    {
        if (densityNoise == null || rule == null)
            return result;

        float density = densityNoise.Sample(location.x, location.y);

        float oceanDepth = density * strength * oceanStrength;

        if (oceanDepth < oceanCutoff)
            return result;

        return rule.Apply(result);
    }
}