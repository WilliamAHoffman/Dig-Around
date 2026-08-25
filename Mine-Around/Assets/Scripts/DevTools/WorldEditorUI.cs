using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldEditorUI : MonoBehaviour
{
    private UIDocument uiDocument;

    private Toggle wallToggle;
    private Toggle floorToggle;

    [SerializeField] private GameDatabase<BloxelBase> tileDataBase;
    [SerializeField] private WorldEditor worldEditor;

    private List<BloxelBase> tiles;
    private List<BloxelBase> wallTiles;
    private List<BloxelBase> floorTiles;

    private ListView walls;
    private ListView floors;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        tiles = new List<BloxelBase>(tileDataBase.Assets);

        wallToggle = root.Q<Toggle>("ToggleWalls");
        floorToggle = root.Q<Toggle>("ToggleFloors");

        wallToggle.RegisterValueChangedCallback(OnWallToggleChanged);
        floorToggle.RegisterValueChangedCallback(OnFloorToggleChanged);

        walls = root.Q<ListView>("Walls");
        floors = root.Q<ListView>("Floors");

        ConfigureLists();

        walls.selectionChanged += OnWallSelected;
        floors.selectionChanged += OnFloorSelected;
    }

    private void ConfigureLists()
    {
        wallTiles = tiles
            .Where(tile =>
                tile != null &&
                tile.SupportsLayer(BloxelLayer.Wall))
            .ToList();

        floorTiles = tiles
            .Where(tile =>
                tile != null &&
                tile.SupportsLayer(BloxelLayer.Floor))
            .ToList();

        ConfigureListView(walls, wallTiles);
        ConfigureListView(floors, floorTiles);

        walls.selectionChanged += OnWallSelected;
        floors.selectionChanged += OnFloorSelected;
    }

    private static void ConfigureListView(
    ListView listView,
    List<BloxelBase> source)
    {
        listView.itemsSource = source;

        listView.makeItem = () =>
        {
            var label = new Label();

            label.style.paddingLeft = 10;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;

            return label;
        };

        listView.bindItem = (element, index) =>
        {
            var label = (Label)element;
            BloxelBase bloxel = source[index];

            label.text = bloxel.NameID;
        };

        listView.unbindItem = (element, index) =>
        {
            // Prevent recycled elements from retaining old text.
            ((Label)element).text = string.Empty;
        };
    }

    private void OnWallToggleChanged(ChangeEvent<bool> evt)
    {
        worldEditor.toggleWalls = evt.newValue;
    }

    private void OnFloorToggleChanged(ChangeEvent<bool> evt)
    {
        worldEditor.toggleFloors = evt.newValue;
    }

    private void OnFloorSelected(IEnumerable<object> selectedItems)
    {
        var selectedFloor = floors.selectedItem as BloxelBase;

        if (selectedFloor == null)
            return;

        worldEditor.selectedFloor = selectedFloor.ID;
    }

    private void OnWallSelected(IEnumerable<object> selectedItems)
    {
        var selectedWall = walls.selectedItem as BloxelBase;

        if (selectedWall == null)
            return;

        worldEditor.selectedWall = selectedWall.ID;
    }

    void OnDisable()
    {
        if (wallToggle != null)
            wallToggle.UnregisterValueChangedCallback(OnWallToggleChanged);

        if (floorToggle != null)
            floorToggle.UnregisterValueChangedCallback(OnFloorToggleChanged);

        if (walls != null)
            walls.selectionChanged -= OnWallSelected;

        if (floors != null)
            floors.selectionChanged -= OnFloorSelected;
    }
}