#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GameDatabaseFiller
{
    public static void PrepareDatabase<T>(GameDatabase<T> database) where T : DatabaseAsset
    {
        FillDatabase<T>(database);

        database.RebuildLookups();
        database.InitializeAssets();
    }

    private static void FillDatabase<T>(GameDatabase<T> database) where T : DatabaseAsset
    {
        if (database == null)
            return;

        List<DatabaseAsset> assets = FindCompatibleAssets(typeof(T));

        Undo.RecordObject(database, "Fill Game Database");

        SerializedObject serializedDatabase = new(database);
        SerializedProperty assetsProperty =
            serializedDatabase.FindProperty("assets");

        if (assetsProperty == null || !assetsProperty.isArray)
        {
            Debug.LogError(
                "Could not find serialized array field 'assets' on " +
                $"'{database.GetType().Name}'.",
                database);
            return;
        }

        assetsProperty.arraySize = assets.Count;

        for (int index = 0; index < assets.Count; index++)
        {
            assetsProperty.GetArrayElementAtIndex(index).objectReferenceValue =
                assets[index];
        }

        serializedDatabase.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);

        Debug.Log(
            $"'{database.name}' filled with {assets.Count} " +
            $"{typeof(T)} asset(s).",
            database);
    }

    private static List<DatabaseAsset> FindCompatibleAssets(Type assetType)
    {
        string[] guids = AssetDatabase.FindAssets("t:DatabaseAsset");
        List<DatabaseAsset> assets = new();
        Dictionary<int, DatabaseAsset> seenIds = new();
        Dictionary<string, DatabaseAsset> seenNames =
            new(StringComparer.Ordinal);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DatabaseAsset asset =
                AssetDatabase.LoadAssetAtPath<DatabaseAsset>(path);

            if (asset == null || !assetType.IsInstanceOfType(asset))
                continue;

            ValidateIdentity(asset, path, seenIds, seenNames);
            assets.Add(asset);
        }

        assets.Sort(CompareAssets);
        return assets;
    }

    private static void ValidateIdentity(
        DatabaseAsset asset,
        string path,
        Dictionary<int, DatabaseAsset> seenIds,
        Dictionary<string, DatabaseAsset> seenNames)
    {
        if (!seenIds.TryAdd(asset.ID, asset))
        {
            Debug.LogError(
                $"Duplicate ID {asset.ID}: '{asset.name}' conflicts with " +
                $"'{seenIds[asset.ID].name}'. Path: {path}",
                asset);
        }

        if (string.IsNullOrWhiteSpace(asset.NameID))
        {
            Debug.LogWarning(
                $"Database asset at '{path}' has an empty NameID.",
                asset);
        }
        else if (!seenNames.TryAdd(asset.NameID, asset))
        {
            Debug.LogError(
                $"Duplicate NameID '{asset.NameID}': '{asset.name}' " +
                $"conflicts with '{seenNames[asset.NameID].name}'. " +
                $"Path: {path}",
                asset);
        }
    }

    private static int CompareAssets(DatabaseAsset left, DatabaseAsset right)
    {
        int idComparison = left.ID.CompareTo(right.ID);

        return idComparison != 0
            ? idComparison
            : string.Compare(left.NameID, right.NameID, StringComparison.Ordinal);
    }
}

#endif