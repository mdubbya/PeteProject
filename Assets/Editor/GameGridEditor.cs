using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameGrid))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameGrid grid = (GameGrid)target;
        if (GUILayout.Button("Reinitialize"))
        {
            grid.Start();
        }
    }
}