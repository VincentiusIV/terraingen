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
    public bool fixedOffset;

    public Vector2 randomOffsetRange = new Vector2(1000f, 1000f);

    public float[,] Generate()
    {
        Vector2 offset = new Vector2(Random.Range(-randomOffsetRange.x, randomOffsetRange.x), Random.Range(-randomOffsetRange.y, randomOffsetRange.y));
        if(fixedOffset)
        {
            offset.x = transform.position.x;
            offset.y = transform.position.z;
        }
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity, falloff, offset, SquareSmoothRange);
        if (!Application.isPlaying)
        {
            NoiseDisplay display = FindObjectOfType<NoiseDisplay>();
            display.drawNoiseMap(noiseMap);
        }
        return noiseMap;
    }
}
