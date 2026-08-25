using Unity.VisualScripting;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public int selectedWall;
    public int selectedFloor;
    public bool toggleWalls;
    public bool toggleFloors;
    [SerializeField] private ChunkManager chunkManager;

    public void SetSelectedCell(Vector2Int position)
    {
        Debug.Log("tried placement: " + position);
        if(selectedWall != -1 && toggleWalls)
        {
            chunkManager.SetBloxelAtWorldPosition(position, selectedWall, BloxelLayer.Wall);
        }
        if(selectedFloor != -1 && toggleFloors)
        {
            chunkManager.SetBloxelAtWorldPosition(position, selectedFloor, BloxelLayer.Floor);
        }
        return;
    }
}
