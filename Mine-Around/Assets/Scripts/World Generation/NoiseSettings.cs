using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Scriptable Objects/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    public int octaves;
    public float persistence;
    public float lacunarity;
    public float scale;
}
