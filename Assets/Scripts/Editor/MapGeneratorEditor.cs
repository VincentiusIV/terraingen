using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        NoiseGenerator mapGen = (NoiseGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.Generate();   
            }
        }
        if (GUILayout.Button("Generate"))
        {
            mapGen.Generate();
        }
    }
}
