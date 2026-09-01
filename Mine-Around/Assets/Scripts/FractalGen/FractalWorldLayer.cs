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
    [SerializeField] private FractalDecorator baseDecor;

    public MapCell Generate(
        Vector2Int location,
        MapCell result)
    {
        //bool modified = false;

        if (baseDecor)
        {
            result = baseDecor.PlaceLayers(result, location);
        }

        if (!noise)
        {
            return result;
        }

        float sample = noise.Sample(location.x, location.y);

        int selectedIndex = -1;
        float highestValidThreshold = float.NegativeInfinity;

        for (int i = 0; i < subLayers.Count; i++)
        {
            float threshold = subLayers[i].left.val;

            if (threshold < sample && threshold > highestValidThreshold)
            {
                highestValidThreshold = threshold;
                selectedIndex = i;
            }
        }

        if (selectedIndex >= 0)
        {
            return subLayers[selectedIndex].right.Generate(location, result);
        }

        return result;
    }
}