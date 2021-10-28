using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainPipeline))]
public class TerrainPipelineEditor : Editor
{
    private TerrainPipeline pipeline;

    public void OnEnable()
    {
        pipeline = target as TerrainPipeline;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Run"))
            pipeline.GenerateTerrain();
    }
}
