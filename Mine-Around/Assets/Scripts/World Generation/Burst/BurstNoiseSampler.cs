using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public static class BurstWorldSampler
{
    public static WorldSampleData Sample(
        int2 worldPosition,
        int seed,
        float heightScale,
        float moistureScale,
        float temperatureScale)
    {
        float2 position = worldPosition;

        float height = noise.snoise(
            position * heightScale + new float2(seed, seed));

        float moisture = noise.snoise(
            position * moistureScale + new float2(seed + 1000, seed + 1000));

        float temperature = noise.snoise(
            position * temperatureScale + new float2(seed + 2000, seed + 2000));

        return new WorldSampleData
        {
            Height = height,
            Moisture = moisture,
            Temperature = temperature
        };
    }
}