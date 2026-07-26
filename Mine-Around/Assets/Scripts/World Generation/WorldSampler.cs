using UnityEngine;

[CreateAssetMenu(fileName = "WorldSampler", menuName = "World Generation/WorldSampler")]
public class WorldSampler : ScriptableObject
{
    public NoiseSettings elevationNoiseSettings;
    public NoiseSettings temperatureNoiseSettings;

    public WorldSample Sample(Vector2Int worldPosition)
    {
        return new WorldSample(
            elevationNoiseSettings.Sample(
                worldPosition.x,
                worldPosition.y
            ),
            temperatureNoiseSettings.Sample(
                worldPosition.x,
                worldPosition.y
            )
        );
    }
}