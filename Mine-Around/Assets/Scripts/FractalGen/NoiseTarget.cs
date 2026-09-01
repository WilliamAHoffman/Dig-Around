using System;
using UnityEngine;

[Serializable]
public class NoiseTarget
{
    [SerializeField] NoiseSettings noise;
    [SerializeField][Range(-1f,1f)] private float noiseTarget = 10;
    [SerializeField] public float strength = 1;

    public float GetStrength(int x, int y)
    {
        float sample = noise.Sample(x,y); 
        
        float sampleTarget = noiseTarget;

        float difference = Math.Abs(sample - sampleTarget);

        return (1 - difference) * strength;
    }
}
