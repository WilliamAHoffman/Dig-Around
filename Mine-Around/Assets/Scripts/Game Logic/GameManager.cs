using UnityEngine;
using System.Diagnostics; //debug

public class GameManager : MonoBehaviour
{
    [SerializeField] bool randomSeed;

    //starting spawn area
    [SerializeField] private int startSizeX;
    [SerializeField] private int startSizeY;
    [SerializeField] private Vector2Int origin;
    [SerializeField] private bool renderStartChunks;

    //external references
    [SerializeField] private GameController gameController;
    public ChunkManager ChunkManager => gameController.ChunkManager;
    public GameVariables GameVariables => gameController.GameVariables;
    public GameDatabase GameDatabase => gameController.GameDatabase;

    public WorldGenerator worldGenerator;
    private void Start()
    {
        SetUpGame();
    }

    public void CreateWorld()
    {

        Stopwatch stopwatch = new Stopwatch(); //debug
        stopwatch.Start(); //debug

        SetUpGame();
        //ChunkManager.DeleteAllChunks();
        ChunkManager.AsyncCreateBox(new Vector2Int(-startSizeX, -startSizeY) + origin, new Vector2Int(startSizeX, startSizeY) + origin, renderStartChunks);

        stopwatch.Stop(); //debug
        UnityEngine.Debug.Log($"Creating the world took: {stopwatch.ElapsedMilliseconds} ms", this); //debug
    }

    public void SetUpGame()
    {

        if (randomSeed) GameVariables.SetNewWorldSeed();

        //ChunkManager.DeleteAllChunks();

        GameDatabase.Initialize();
    }

    public void DeleteWorld()
    {
        ChunkManager.DeleteAllChunks();
    }

    public void UnloadWorld()
    {
        ChunkManager.UnloadAllChunks();
    }
}