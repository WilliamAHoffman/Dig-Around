#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility for automatically filling GameDatabase.allAssets from
/// DatabaseAsset ScriptableObject assets in the Unity project.
///
/// Put this file in an Editor folder, for example:
/// Assets/Scripts/Editor/GameDatabaseEditor.cs
/// </summary>
[CustomEditor(typeof(GameDatabase))]
public class GameDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        GameDatabase database = (GameDatabase)target;

        if (GUILayout.Button("Find DatabaseAsset Assets"))
        {
            FillDatabase(database);
        }

        if (GUILayout.Button("Find DatabaseAsset Assets And Initialize"))
        {
            FillDatabase(database);
            database.Initialize();
        }
    }

    [MenuItem("Tools/World Generation/Fill Selected DatabaseAsset Database")]
    private static void FillSelectedDatabase()
    {
        GameDatabase database = Selection.activeGameObject != null
            ? Selection.activeGameObject.GetComponent<GameDatabase>()
            : null;

        if (database == null)
        {
            Debug.LogWarning("Select a GameObject that has an GameDatabase component.");
            return;
        }

        FillDatabase(database);
        database.Initialize();
    }

    [MenuItem("Tools/World Generation/Fill All DatabaseAsset Databases In Open Scenes")]
    private static void FillAllDatabasesInOpenScenes()
    {
        GameDatabase[] databases = FindObjectsByType<GameDatabase>(FindObjectsSortMode.None);

        if (databases.Length == 0)
        {
            Debug.LogWarning("No GameDatabase components were found in the open scenes.");
            return;
        }

        foreach (GameDatabase database in databases)
        {
            FillDatabase(database);
            database.Initialize();
        }

        Debug.Log($"Filled {databases.Length} GameDatabase component(s).");
    }

    private static void FillDatabase(GameDatabase database)
    {
        if (database == null)
            return;

        List<DatabaseAsset> assets = FindAllDatabaseAssetAssets();

        Undo.RecordObject(database, "Fill DatabaseAsset Database");

        SerializedObject serializedDatabase = new SerializedObject(database);
        SerializedProperty allAssetsProperty = serializedDatabase.FindProperty("allAssets");

        if (allAssetsProperty == null || !allAssetsProperty.isArray)
        {
            Debug.LogError("Could not find serialized field 'allAssets' on GameDatabase.", database);
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

        Debug.Log($"GameDatabase filled with {assets.Count} DatabaseAsset asset(s).", database);
    }

    private static List<DatabaseAsset> FindAllDatabaseAssetAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:DatabaseAsset");
        List<DatabaseAsset> assets = new List<DatabaseAsset>();
        HashSet<string> seenIDs = new HashSet<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DatabaseAsset asset = AssetDatabase.LoadAssetAtPath<DatabaseAsset>(path);

            if (asset == null)
                continue;

            if (string.IsNullOrWhiteSpace(asset.nameID))
            {
                Debug.LogWarning($"DatabaseAsset asset at '{path}' has an empty nameID and was still added. Consider assigning a nameID.", asset);
            }
            else if (!seenIDs.Add(asset.nameID))
            {
                Debug.LogError($"Duplicate DatabaseAsset nameID found while scanning: '{asset.nameID}'. Asset path: {path}", asset);
            }

            assets.Add(asset);
        }

        assets.Sort((a, b) => string.Compare(a.nameID, b.nameID, System.StringComparison.Ordinal));
        return assets;
    }
}
#endif