using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Scriptable Objects/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    [SerializeField] public FastNoiseLite.NoiseType noiseType;

    public FastNoiseLite GetNoise(int seed)
    {
        FastNoiseLite noise = new FastNoiseLite(seed);
        noise.SetNoiseType(noiseType);

        return noise;
    }
}
