using UnityEngine;

public enum WorldLayerType
{
    Elevation,
    Temperature,
    Moisture,
    Caves,
    Detail,
    BiomeMask
}

[CreateAssetMenu(fileName = "WorldLayer", menuName = "Scriptable Objects/WorldLayer")]
public class WorldLayer : WorldDataObject
{
    public WorldLayerType layerType;
    public NoiseSettings noiseSettings;

    public float Sample(Vector2Int worldPos)
    {
        if (noiseSettings == null)
            return 0f;

        return noiseSettings.Sample(worldPos.x, worldPos.y);
    }
}