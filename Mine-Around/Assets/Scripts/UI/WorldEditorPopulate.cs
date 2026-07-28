using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldEditorPopulate : MonoBehaviour
{
    private UIDocument uiDocument;
    private ListView listView;
    private List<string> listData;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        
        // 1. Generate data for the list
        listData = new List<string> { "Warrior", "Mage", "Rogue", "Paladin", "Cleric" };

        // 2. Locate the ListView inside the UXML
        listView = uiDocument.rootVisualElement.Q<ListView>();

        // 3. Assign the data source
        listView.itemsSource = listData;

        // 4. Define how to CREATE an individual item element slot
        listView.makeItem = () => new Label();

        // 5. Define how to BIND data to that created item element slot
        listView.bindItem = (visualElement, index) => 
        {
            var label = visualElement as Label;
            label.text = listData[index];
            label.style.paddingLeft = 10; // Basic styling via code
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
        };

        // 6. Hook up the selection events
        listView.selectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(IEnumerable<object> selectedItems)
    {
        // Get the chosen item safely
        foreach (var item in selectedItems)
        {
            Debug.Log($"Selected character class: {item}");
        }
    }

    void OnDisable()
    {
        if (listView != null)
        {
            listView.selectionChanged -= OnSelectionChanged;
        }
    }
}
