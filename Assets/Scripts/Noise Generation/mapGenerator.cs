using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width, height;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;

    public bool autoUpdate;

    public void generateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity);
        mapDisplay display = FindObjectOfType<mapDisplay>();
        display.drawNoiseMap(noiseMap);
    }
}
