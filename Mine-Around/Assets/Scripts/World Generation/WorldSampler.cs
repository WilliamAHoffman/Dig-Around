using UnityEngine;

[CreateAssetMenu(fileName = "WorldSampler", menuName = "World Generation/WorldSampler")]
public class WorldSampler : ScriptableObject
{
    public NoiseSettings elevationNoiseSettings;
    public NoiseSettings temperatureNoiseSettings;

    public WorldSample Sample(Vector2Int worldPos)
    {
        WorldSample sample = new WorldSample();

        sample.Add(WorldSampleType.elevation, elevationNoiseSettings.Sample(worldPos.x, worldPos.y));
        sample.Add(WorldSampleType.temperature, temperatureNoiseSettings.Sample(worldPos.x, worldPos.y));
        
        return sample;
    }
}