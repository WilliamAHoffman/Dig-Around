using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldPreviewer))]
public class WorldPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldPreviewer worldPreviewer = (WorldPreviewer)target;

        if (GUILayout.Button("Preview World"))
        {
            worldPreviewer.PreviewBox();
        }
    }
}