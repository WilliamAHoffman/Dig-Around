using UnityEngine;

public class Noise
{
    private readonly int seed;
    private readonly float offsetX;
    private readonly float offsetY;

    public Noise(int seed)
    {
        this.seed = seed;

        System.Random prng = new System.Random(seed);
        offsetX = prng.Next(-100000, 100000);
        offsetY = prng.Next(-100000, 100000);
    }

    public float SampleNoise(float x, float y, int octaves, float persistence, float lacunarity, float scale)
    {
        scale = Mathf.Max(0.0001f, scale);
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;
        float maxPossibleHeight = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offsetX) / scale * frequency;
            float sampleY = (y + offsetY) / scale * frequency;

            float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

            noiseHeight += perlin * amplitude;
            maxPossibleHeight += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight / maxPossibleHeight;
    }

    public float SampleNoise(float x, float y, NoiseSettings noiseSettings)
    {
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