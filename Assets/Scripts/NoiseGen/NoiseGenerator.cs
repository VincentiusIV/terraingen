using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int width, height;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float falloff;
    public int SquareSmoothRange = 5;
    public bool autoUpdate;

    public float[,] Generate()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity, falloff, new Vector2(transform.position.x, transform.position.z), SquareSmoothRange);
        if (!Application.isPlaying)
        {
            NoiseDisplay display = FindObjectOfType<NoiseDisplay>();
            display.drawNoiseMap(noiseMap);
        }
        return noiseMap;
    }
}
