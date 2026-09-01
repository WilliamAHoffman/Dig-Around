using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Fractal Decorator",
    menuName = "FractalGen/Decorator"
)]
public class FractalDecorator : ScriptableObject
{
    public List<DecorationLayer> decorationLayers;


    public MapCell PlaceLayers(MapCell result, Vector2Int location)
    {
        foreach (DecorationLayer decorationLayer in decorationLayers)
        {
            result = decorationLayer.ModifyCell(result, location);
        }

        return result;
    }
}
