using UnityEngine;

[CreateAssetMenu(
    fileName = "WorldSampler",
    menuName = "World Generation/World Sampler"
)]
public class WorldSampler : ScriptableObject
{
    [Header("Noise Settings")]
    [SerializeField]
    private NoiseSettings elevationNoiseSettings;

    [SerializeField]
    private NoiseSettings temperatureNoiseSettings;

    public WorldSample Sample(
        Vector2Int worldPosition)
    {
        if (!ValidateSettings())
        {
            return default;
        }

        float elevation =
            elevationNoiseSettings.Sample(
                worldPosition.x,
                worldPosition.y
            );

        float temperature =
            temperatureNoiseSettings.Sample(
                worldPosition.x,
                worldPosition.y
            );

        return new WorldSample(
            elevation,
            temperature
        );
    }

    private bool ValidateSettings()
    {
        if (elevationNoiseSettings == null)
        {
            Debug.LogError(
                $"{name} is missing elevation noise settings.",
                this
            );

            return false;
        }

        if (temperatureNoiseSettings == null)
        {
            Debug.LogError(
                $"{name} is missing temperature noise settings.",
                this
            );

            return false;
        }

        return true;
    }
}