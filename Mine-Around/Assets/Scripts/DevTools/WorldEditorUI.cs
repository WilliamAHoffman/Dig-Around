using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldEditorUI : MonoBehaviour
{
    private UIDocument uiDocument;

    private Toggle wallToggle;
    private Toggle floorToggle;

    [SerializeField] private GameDatabase<BloxelBase> tileDataBase;
    [SerializeField] private WorldEditor worldEditor;

    private IReadOnlyList<BloxelBase> tiles;

    private ListView walls;
    private ListView floors;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        tiles = tileDataBase.Assets;

        wallToggle = root.Q<Toggle>("ToggleWalls");
        floorToggle = root.Q<Toggle>("ToggleFloors");

        wallToggle.RegisterValueChangedCallback(OnWallToggleChanged);
        floorToggle.RegisterValueChangedCallback(OnFloorToggleChanged);

        walls = root.Q<ListView>("Walls");
        floors = root.Q<ListView>("Floors");

        walls.itemsSource = new List<BloxelBase>(tiles);
        floors.itemsSource = new List<BloxelBase>(tiles);

        walls.makeItem = () => new Label();
        floors.makeItem = () => new Label();

        walls.bindItem = (visualElement, index) =>
        {
            var label = visualElement as Label;

            label.text = tiles[index].NameID;
            label.style.paddingLeft = 10;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
        };

        floors.bindItem = (visualElement, index) =>
        {
            var label = visualElement as Label;

            label.text = tiles[index].NameID;
            label.style.paddingLeft = 10;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
        };

        walls.selectionChanged += OnWallSelected;
        floors.selectionChanged += OnFloorSelected;
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