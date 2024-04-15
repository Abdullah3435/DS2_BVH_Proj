using UnityEngine;
using TMPro;
using UnityEditor;

[ExecuteInEditMode]
public class EditorFPSDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            Debug.LogWarning("TextMeshProUGUI component not found on GameObject.");
            return;
        }
    }

    void Update()
    {
        if (textMeshPro != null)
        {
            float fps = 1f / Time.deltaTime;
            textMeshPro.text = "FPS: " + Mathf.Round(fps);
        }
    }
}

[CustomEditor(typeof(EditorFPSDisplay))]
public class EditorFPSDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying)
        {
            GUILayout.Label("FPS is displayed only in Play mode.");
        }
    }
}