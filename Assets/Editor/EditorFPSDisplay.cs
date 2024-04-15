using UnityEditor;
using UnityEngine;

public class StatsWindow : EditorWindow
{
    [MenuItem("Window/Stats Window")]
    public static void ShowWindow()
    {
        GetWindow<StatsWindow>("Stats");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Stats", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Display FPS
        EditorGUILayout.LabelField("FPS", Mathf.RoundToInt(1f / Time.deltaTime).ToString());

        // Add more stats here as needed
    }
}
