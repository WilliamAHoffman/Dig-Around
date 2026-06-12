using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int worldSeed;
    bool randomSeed;

    public WorldGenerator worldGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        CreateWorld();
    }
    public void CreateWorld()
    {
        if (randomSeed)
        {
            worldSeed = Random.Range(-1000,1000);
        }
        worldGenerator.SetNoise(worldSeed);
        WorldDataRegistry.Instance.SetFeatureNoise(worldSeed);

        ChunkManager.Instance.CreateRadiusAt(new Vector2Int(0,0));
    }
}