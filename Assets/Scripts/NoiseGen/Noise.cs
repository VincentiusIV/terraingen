using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise { 
    public static float [,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, float falloff)
    {
        float[,] noiseMap = new float[width, height];

        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                if(falloff <= 0)
                {
                    falloff = 0.1f;
                }

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                float distToCenter = Mathf.Sqrt( (Mathf.Pow(Mathf.Abs(x - (width / 2)), 2) + Mathf.Pow(Mathf.Abs(y - (height / 2)), 2)) ) + Random.Range(-0.9f, 1.1f) + noiseHeight;
                distToCenter = NormalizeDist(distToCenter, width, height);
                //Debug.Log(string.Format("Dist To Center {0}", distToCenter));

                if (distToCenter > falloff)
                {
                    noiseMap[x, y] = noiseHeight + (2 * minNoiseHeight);
                  //if (noiseMap[x, y] > maxNoiseHeight) maxNoiseHeight = noiseMap[x, y];
                }
                else
                {
                    noiseMap[x, y] = noiseHeight;
                }
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        //Debug.Log(string.Format("{0} --- {1}", minNoiseHeight, maxNoiseHeight));
        return noiseMap;
    }
        
    private static float NormalizeDist(float distance, int width, int height)
    {
        float a1 = 0;
        float a2 = Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2))/2;
        float b1 = 0;
        float b2 = 1;
        float normDistance = b1 + (distance - a1) * (b2 - b1) / (a2 - a1);
        return normDistance;
    }
}
