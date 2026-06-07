using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "Scriptable Objects/GenerationRule")]
public class GenerationRule : ScriptableObject
{
    public string tileID;

    public List<string> replaces;

    [Range(0f, 1f)]
    public float minNoiseValue;

    [Range(0f, 1f)]
    public float maxNoiseValue = 1f;

    public bool Matches(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }

    public bool Replaces(string replacing)
    {
        if(replaces.Count == 0) return true;
        return replaces.Contains(replacing);
    }
}