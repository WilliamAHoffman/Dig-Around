#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility for automatically filling WorldDataObjectDatabase.allAssets from
/// WorldDataObject ScriptableObject assets in the Unity project.
///
/// Put this file in an Editor folder, for example:
/// Assets/Scripts/Editor/WorldDataObjectDatabaseEditor.cs
/// </summary>
[CustomEditor(typeof(WorldDataObjectDataBase))]
public class WorldDataObjectDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        WorldDataObjectDataBase database = (WorldDataObjectDataBase)target;

        if (GUILayout.Button("Find WorldDataObject Assets"))
        {
            FillDatabase(database);
        }

        if (GUILayout.Button("Find WorldDataObject Assets And Initialize"))
        {
            FillDatabase(database);
            database.Initialize();
        }
    }

    [MenuItem("Tools/World Generation/Fill Selected WorldDataObject Database")]
    private static void FillSelectedDatabase()
    {
        WorldDataObjectDataBase database = Selection.activeGameObject != null
            ? Selection.activeGameObject.GetComponent<WorldDataObjectDataBase>()
            : null;

        if (database == null)
        {
            Debug.LogWarning("Select a GameObject that has an WorldDataObjectDatabase component.");
            return;
        }

        FillDatabase(database);
        database.Initialize();
    }

    [MenuItem("Tools/World Generation/Fill All WorldDataObject Databases In Open Scenes")]
    private static void FillAllDatabasesInOpenScenes()
    {
        WorldDataObjectDataBase[] databases = FindObjectsByType<WorldDataObjectDataBase>(FindObjectsSortMode.None);

        if (databases.Length == 0)
        {
            Debug.LogWarning("No WorldDataObjectDatabase components were found in the open scenes.");
            return;
        }

        foreach (WorldDataObjectDataBase database in databases)
        {
            FillDatabase(database);
            database.Initialize();
        }

        Debug.Log($"Filled {databases.Length} WorldDataObjectDatabase component(s).");
    }

    private static void FillDatabase(WorldDataObjectDataBase database)
    {
        if (database == null)
            return;

        List<WorldDataObject> assets = FindAllWorldDataObjectAssets();

        Undo.RecordObject(database, "Fill WorldDataObject Database");

        SerializedObject serializedDatabase = new SerializedObject(database);
        SerializedProperty allAssetsProperty = serializedDatabase.FindProperty("allAssets");

        if (allAssetsProperty == null || !allAssetsProperty.isArray)
        {
            Debug.LogError("Could not find serialized field 'allAssets' on WorldDataObjectDatabase.", database);
            return;
        }

        allAssetsProperty.ClearArray();

        for (int i = 0; i < assets.Count; i++)
        {
            allAssetsProperty.InsertArrayElementAtIndex(i);
            allAssetsProperty.GetArrayElementAtIndex(i).objectReferenceValue = assets[i];
        }

        serializedDatabase.ApplyModifiedProperties();

        EditorUtility.SetDirty(database);

        Debug.Log($"WorldDataObjectDatabase filled with {assets.Count} WorldDataObject asset(s).", database);
    }

    private static List<WorldDataObject> FindAllWorldDataObjectAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:WorldDataObject");
        List<WorldDataObject> assets = new List<WorldDataObject>();
        HashSet<string> seenIDs = new HashSet<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WorldDataObject asset = AssetDatabase.LoadAssetAtPath<WorldDataObject>(path);

            if (asset == null)
                continue;

            if (string.IsNullOrWhiteSpace(asset.nameID))
            {
                Debug.LogWarning($"WorldDataObject asset at '{path}' has an empty nameID and was still added. Consider assigning a nameID.", asset);
            }
            else if (!seenIDs.Add(asset.nameID))
            {
                Debug.LogError($"Duplicate WorldDataObject nameID found while scanning: '{asset.nameID}'. Asset path: {path}", asset);
            }

            assets.Add(asset);
        }

        assets.Sort((a, b) => string.Compare(a.nameID, b.nameID, System.StringComparison.Ordinal));
        return assets;
    }
}
#endif
