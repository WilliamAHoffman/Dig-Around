using UnityEngine;

[CreateAssetMenu(fileName = "GameVariables", menuName = "Storage/GameVariables")]
public class GameVariables : ScriptableObject
{
    public int worldSeed;
    public int chunkSize;
    public float worldFrequencyScale;

    public void SetNewWorldSeed()
    {
        worldSeed = Random.Range(int.MinValue, int.MaxValue);
    }
}
