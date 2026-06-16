using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Scriptable Objects/NoiseSettings")]
public class NoiseSettings : WorldDataObject
{
    private FastNoiseLite noise;
    private FastNoiseLite warpNoise;

    [Header("General")]
    [SerializeField] private FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
    [Min(0f)]
    [SerializeField] private float frequency = 0.01f;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    [Header("Fractal")]
    [SerializeField] private FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.None;
    [Min(1)]
    [SerializeField] private int fractalOctaves = 3;
    [Min(0f)]
    [SerializeField] private float fractalLacunarity = 2f;
    [SerializeField] private float fractalGain = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float fractalWeightedStrength = 0f;
    [Min(0f)]
    [SerializeField] private float fractalPingPongStrength = 2f;

    [Header("Cellular")]
    [SerializeField] private FastNoiseLite.CellularDistanceFunction cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
    [SerializeField] private FastNoiseLite.CellularReturnType cellularReturnType = FastNoiseLite.CellularReturnType.Distance;
    [Range(0f, 1f)]
    [SerializeField] private float cellularJitter = 1f;

    [Header("Domain Warp")]
    [SerializeField] private bool domainWarp;
    [SerializeField] private FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2;
    [Min(0f)]
    [SerializeField] private float domainWarpAmp = 1f;
    [Min(0f)]
    [SerializeField] private float domainWarpFrequency = 0.01f;

    [Header("Domain Warp Fractal")]
    [SerializeField] private FastNoiseLite.FractalType domainWarpFractal = FastNoiseLite.FractalType.None;
    [Min(1)]
    [SerializeField] private int domainWarpOctaves = 3;
    [Min(0f)]
    [SerializeField] private float domainWarpLacunarity = 2f;
    [SerializeField] private float domainWarpGain = 0.5f;

    /// <summary>
    /// Creates a FastNoiseLite instance configured for regular noise sampling.
    /// </summary>
    public void CreateNoise(int baseSeed)
    {

        int regularSeed = baseSeed ^ WorldDataObjectRandomness.StableHash(nameID + "_regular");
        int warpSeed = baseSeed ^ WorldDataObjectRandomness.StableHash(nameID + "_warp");

        noise = new FastNoiseLite(regularSeed);
        warpNoise = new FastNoiseLite(warpSeed);

        offsetX = WorldDataObjectRandomness.StableHash(nameID + "_offset_x") % 10000;
        offsetY = WorldDataObjectRandomness.StableHash(nameID + "_offset_y") % 10000;

        ApplyNoiseSettings(noise);
        ApplyDomainWarpSettings(warpNoise);
    }
    /// <summary>
    /// Creates a separate FastNoiseLite instance configured for DomainWarp calls.
    /// FastNoiseLite uses one shared frequency/fractal state, so domain warp settings
    /// are kept separate from regular noise settings by using a separate instance.
    /// </summary>

    public void ApplyNoiseSettings(FastNoiseLite noise)
    {
        noise.SetNoiseType(noiseType);
        noise.SetFrequency(frequency);

        noise.SetFractalType(fractalType);
        noise.SetFractalOctaves(Mathf.Max(1, fractalOctaves));
        noise.SetFractalLacunarity(fractalLacunarity);
        noise.SetFractalGain(fractalGain);
        noise.SetFractalWeightedStrength(fractalWeightedStrength);
        noise.SetFractalPingPongStrength(fractalPingPongStrength);

        noise.SetCellularDistanceFunction(cellularDistanceFunction);
        noise.SetCellularReturnType(cellularReturnType);
        noise.SetCellularJitter(cellularJitter);
    }

    public void ApplyDomainWarpSettings(FastNoiseLite noise)
    {
        noise.SetDomainWarpType(domainWarpType);
        noise.SetDomainWarpAmp(domainWarpAmp);
        noise.SetFrequency(domainWarpFrequency);

        noise.SetFractalType(domainWarpFractal);
        noise.SetFractalOctaves(Mathf.Max(1, domainWarpOctaves));
        noise.SetFractalLacunarity(domainWarpLacunarity);
        noise.SetFractalGain(domainWarpGain);
    }

    public float Sample(int worldX, int worldY)
    {
        float x = worldX + offsetX;
        float y = worldY + offsetY;

        if (domainWarp)
        {
            if (warpNoise == null)
            {
                Debug.LogError($"NoiseSettings '{nameID}' has domain warp enabled but warpNoise was not created.", this);
                return noise.GetNoise(x, y);
            }

            warpNoise.DomainWarp(ref x, ref y);
        }

        return noise.GetNoise(x, y);
    }
}
