using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FactalWorldLayer",
    menuName = "FractalGen/WorldLayer"
)]
public class FactalWorldLayer : ScriptableObject
{
    [SerializeField] private List<SerializablePair<RangedFloat, FactalWorldLayer>> subLayers;
    [SerializeField] private NoiseSettings noise;
    [SerializeField] private BloxelBase floor;
    [SerializeField] private BloxelBase wall;
    
    public MapCell Generate(
        Vector2Int location,
        MapCell result)
    {

        if(noise == null)
        {
            return Apply(result);
        }

        float sample = noise.Sample(location.x, location.y);

        if (subLayers == null || subLayers.Count == 0)
        {
            return Apply(result);
        }

        int lowestIndex = 0;
        for(int i = 0; i < subLayers.Count; i++)
        {
            if(subLayers[i].left.val < sample && subLayers[i].left.val > subLayers[lowestIndex].left.val)
            {
                lowestIndex = i;
            }
        }

        return subLayers[lowestIndex].right.Generate(location, result);

    }

    private MapCell Apply(MapCell result)
    {
        if(floor){
            result.FloorID = floor.ID;
        }
        if(wall){
            result.WallID = wall.ID;
        }

        return result;
    }
}