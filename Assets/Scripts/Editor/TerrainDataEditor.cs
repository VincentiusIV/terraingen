using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainData))]
public class TerrainDataEditor : Editor
{
    TerrainData terrainData;

    public void OnEnable()
    {
        terrainData = target as TerrainData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        for (int i = 1; i < terrainData.materials.Length + 1; i++)
        {
            if(terrainData.materials[i - 1].index != i)
            {
                terrainData.materials[i - 1].index = i;
                EditorUtility.SetDirty(terrainData);
            }
        }
    }
}
