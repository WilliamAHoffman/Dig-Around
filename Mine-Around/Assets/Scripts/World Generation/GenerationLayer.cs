using System;
using System.Collections.Generic;
using UnityEngine;
//This class represents a single generation feature, like a biome, ore, tree, river...
[CreateAssetMenu(fileName = "GenerationLayer", menuName = "Scriptable Objects/GenerationLayer")]
public class GenerationLayer : WorldDataObject
{
    public List<GenerationLayer> subLayers;
    public TileData baseTile;
    public NoiseSettings noise;
    public TileData GenerateTile(TileData currentTile, Vector2Int location)
    {
        float noiseSample = noise.Sample(location.x, location.y);

        if (subLayers == null || subLayers.Count == 0)
        {
            return baseTile;
        }

        foreach(GenerationLayer generationLayer in subLayers)
        {
            currentTile = generationLayer.GenerateTile(baseTile, location);
        }

        return currentTile;
    }
}

