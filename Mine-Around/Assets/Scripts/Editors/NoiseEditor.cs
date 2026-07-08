using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseSettings))]
public class NoiseSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoiseSettings Noise = (NoiseSettings)target;

        if (GUILayout.Button("Resfresh Noise"))
        {
            Noise.CreateNoise();
        }
    }
}
