﻿using System.Collections;
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
    public bool useFixedOffset;
    public Vector2 fixedOffset;
    public Vector2 randomOffsetRange;

    public float[,] Generate()
    {
        if(Application.isPlaying)
        {
            useFixedOffset = (Loader.noiseOffset != Vector2.zero);
            fixedOffset = Loader.noiseOffset;
        }
        if (Loader.noiseP != 0) persistance = Loader.noiseP;
        if (Loader.noiseL != 0) lacunarity = Loader.noiseL;
        Vector2 offset = new Vector2(Random.Range(-randomOffsetRange.x, randomOffsetRange.x), Random.Range(-randomOffsetRange.y, randomOffsetRange.y));
        if(useFixedOffset)
        {
            offset = fixedOffset;
        }
        Debug.LogFormat("Noise offset: ({0},{1})", offset.x, offset.y);
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity, falloff, offset, SquareSmoothRange);
        NoiseDisplay display = FindObjectOfType<NoiseDisplay>();
        display.drawNoiseMap(noiseMap);
        return noiseMap;
    }
}
