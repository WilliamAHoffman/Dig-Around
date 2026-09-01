using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering;

[Serializable]
public class DecorationLayer
{
    [SerializeField] private List<NoiseTarget> noiseTargets;
    [SerializeField][Range(0,1)] float requiredSimilairty;
    [SerializeField] private bool probabilistic;
    [SerializeField] private BloxelBase floor;
    [SerializeField] private BloxelBase wall;

    public MapCell ModifyCell(MapCell result, Vector2Int location)
    {
        float totalStrength = 0;
        float currStrength = 0;

        //bool modified = false;

        foreach(NoiseTarget noiseTarget in noiseTargets)
        {
            totalStrength += noiseTarget.strength;
            currStrength += noiseTarget.GetStrength(location.x, location.y);
        }

        if(totalStrength == 0 || currStrength/totalStrength >= requiredSimilairty)
        {
            result.Apply(floor,wall);
        }

        return result;
    }
}
