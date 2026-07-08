using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int worldSeed;
    public bool seedSet = false;
    [SerializeField] bool randomSeed;
    [SerializeField] private int maxSeed;
    [SerializeField] private int minSeed;
    [SerializeField] private int startSizeX;
    [SerializeField] private int startSizeY;
    [SerializeField] private bool renderStartChunks;
    [SerializeField] private bool previewStartChunks;
    [SerializeField] private ChunkPreviewer chunkPreviewer;

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

    private int GetNewSeed()
    {
        return Random.Range(minSeed,maxSeed);
    }

    public void CreateWorld()
    {
        DeleteWorld();

        if (randomSeed)
            worldSeed = GetNewSeed();

        WorldDataObjectDataBase.Instance.Initialize();

        foreach (NoiseSettings noise in WorldDataObjectDataBase.Instance.GetAllAssetsOfType<NoiseSettings>())
        {
            noise.CreateNoise(worldSeed);
        }

        if (previewStartChunks)
        {
            chunkPreviewer.PreviewBox(new Vector2Int(0, 0), new Vector2Int(startSizeX, startSizeY), renderStartChunks);
        }
        else
        {
            ChunkManager.Instance.CreateBox(new Vector2Int(0, 0), new Vector2Int(startSizeX, startSizeY), renderStartChunks);
        }
    }

    public void DeleteWorld()
    {
        ChunkManager.Instance.DeleteAllChunks();
    }
}