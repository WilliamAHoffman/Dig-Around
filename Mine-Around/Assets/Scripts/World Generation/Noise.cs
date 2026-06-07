using UnityEngine;

public static class Noise
{
    public static float SampleNoise(float x, float y, NoiseSettings noiseSettings, int seed)
    {
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);

        float scale = Mathf.Max(0.0001f, noiseSettings.scale);
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;
        float maxPossibleHeight = 0f;

        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            float sampleX = (x + offsetX) / scale * frequency;
            float sampleY = (y + offsetY) / scale * frequency;

            float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

            noiseHeight += perlin * amplitude;
            maxPossibleHeight += amplitude;

            amplitude *= noiseSettings.persistence;
            frequency *= noiseSettings.lacunarity;
        }

        return noiseHeight / maxPossibleHeight;
    }
}