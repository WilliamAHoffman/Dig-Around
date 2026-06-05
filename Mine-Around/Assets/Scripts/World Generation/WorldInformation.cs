using UnityEngine;

[CreateAssetMenu(fileName = "WorldSettings", menuName = "World/World Settings")]
public class WorldSettings : ScriptableObject
{
    [Min(1)] public int chunkSize = 16;
    public int worldSeed = 0;
}