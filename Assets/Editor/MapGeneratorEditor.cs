using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(mapGenerator))]
public class MapGeneratorEditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        mapGenerator mapGen = (mapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.generateMap();   
            }
        }
        if (GUILayout.Button("Generate"))
        {
            mapGen.generateMap();
        }
    }
}
