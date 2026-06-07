using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "Scriptable Objects/GenerationRule")]
public class GenerationRule : ScriptableObject
{
    public WorldTile wallData;

    [Range(0f, 1f)]
    public float minNoiseValue;

    [Range(0f, 1f)]
    public float maxNoiseValue = 1f;

    public bool Matches(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }
}