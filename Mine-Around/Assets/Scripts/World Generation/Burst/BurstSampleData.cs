using Unity.Collections;
using Unity.Mathematics;

public struct WorldSampleData
{
    public float Height;
    public float Moisture;
    public float Temperature;

    public float GetValue(GenerationLayerType type)
    {
        switch (type)
        {
            case GenerationLayerType.Height:
                return Height;

            case GenerationLayerType.Moisture:
                return Moisture;

            case GenerationLayerType.Temperature:
                return Temperature;

            default:
                return 0;
        }
    }
}

public struct GenerationResultData
{
    public int GroundTileId;
    public int FeatureTileId;
}

public enum GenerationLayerType : byte
{
    Height,
    Temperature,
    Moisture
}

public struct GenerationFeatureData
{
    public NativeArray<IdealSample> Samples;
    public GenerationResultData Result;
}

public struct IdealSample
{
    public GenerationLayerType Type;

    public float Value;
    public float Importance;
    public bool Strict;

    public int Seed;
}