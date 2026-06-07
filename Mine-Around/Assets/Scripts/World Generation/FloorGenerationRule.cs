using UnityEngine;

[System.Serializable]
public class FloorGenerationRule
{
    public WorldFloorTile floorData;

    [Range(0f, 1f)]
    public float minNoiseValue;

    [Range(0f, 1f)]
    public float maxNoiseValue = 1f;

    public bool Matches(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }
}