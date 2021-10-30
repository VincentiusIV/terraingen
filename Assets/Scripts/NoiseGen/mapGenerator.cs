using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width, height;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float falloff;

    public bool autoUpdate;

    public float[,] Generate()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity, falloff, new Vector2(transform.position.x, transform.position.z));
        if (!Application.isPlaying)
        {
            MapDisplay display = FindObjectOfType<MapDisplay>();
            display.drawNoiseMap(noiseMap);
        }
        return noiseMap;
    }
}
