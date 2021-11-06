using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, float falloff, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];
        float cuttOff = falloff * 2;
        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                if (falloff <= 0)
                {
                    falloff = 0.1f;
                }

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + offset.x;
                    float sampleY = y / scale * frequency + offset.y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);

                float distToCenter = Mathf.Sqrt((Mathf.Pow(Mathf.Abs(x - (width / 2)), 2) + Mathf.Pow(Mathf.Abs(y - (height / 2)), 2))) + Random.Range(-0.9f, 1.1f) + noiseHeight;
                distToCenter = NormalizeDist(distToCenter, width, height);
                //Debug.Log(string.Format("Dist To Center {0}", distToCenter));

                if (distToCenter < cuttOff)
                {
                    noiseMap[x, y] = noiseHeight * (1 + distToCenter);
                    //if (noiseMap[x, y] > maxNoiseHeight) maxNoiseHeight = noiseMap[x, y];
                }
                else if (distToCenter > falloff && distToCenter > cuttOff)
                {
                    noiseMap[x, y] = noiseHeight * Mathf.Pow((1 + distToCenter), 2);
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
        Debug.Log(string.Format("{0} --- {1}", minNoiseHeight, maxNoiseHeight));
        return noiseMap;
    }

    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, float falloff, Vector2 offset, int SquareSmoothRange)
    {
        float[,] noiseMap = new float[width, height];
        float cuttOff = falloff * 2;
        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                if (falloff <= 0)
                {
                    falloff = 0.1f;
                }

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x + offset.x) / scale * frequency;
                    float sampleY = (y + offset.y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);

                float distToCenter = Mathf.Sqrt((Mathf.Pow(Mathf.Abs(x - (width / 2)), 2) + Mathf.Pow(Mathf.Abs(y - (height / 2)), 2))) + Random.Range(-0.9f, 1.1f) + noiseHeight;
                distToCenter = NormalizeDist(distToCenter, width, height);
                //Debug.Log(string.Format("Dist To Center {0}", distToCenter));

                if (distToCenter < cuttOff)
                {
                    noiseMap[x, y] = noiseHeight * (1 + distToCenter);
                    //if (noiseMap[x, y] > maxNoiseHeight) maxNoiseHeight = noiseMap[x, y];
                }
                else if (distToCenter > falloff && distToCenter > cuttOff)
                {
                    noiseMap[x, y] = noiseHeight * Mathf.Pow((1 + distToCenter), 2);
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
       // Debug.Log(string.Format("{0} --- {1}", minNoiseHeight, maxNoiseHeight));
        noiseMap = SmoothNoise(SquareSmoothRange, noiseMap, width, height, maxNoiseHeight);
        Debug.Log(noiseMap[50, 50]);
        Debug.Log(noiseMap[0, 0]);
        Debug.Log(maxNoiseHeight);
        return noiseMap;
    }

    private static float NormalizeDist(float distance, int width, int height)
    {
        float a1 = 0;
        float a2 = Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)) / 2;
        float b1 = 0;
        float b2 = 1;
        float normDistance = b1 + (distance - a1) * (b2 - b1) / (a2 - a1);
        return normDistance;
    }

    private static float[,] SmoothNoise(int squareRange, float[,] noiseMap, int width, int heigth, float maxVal)
    {
        HashSet<Vector2Int> visitedCoords = new HashSet<Vector2Int>();
        Debug.LogFormat("Smoothing with Range: {0}", squareRange);
        for (int i = 0; i < width * heigth * 3; i++)
        {
            Vector2Int coord = new Vector2Int((int)Random.Range(squareRange, width), (int)Random.Range(squareRange, heigth));
            if (!visitedCoords.Contains(coord))
            {
                visitedCoords.Add(coord);
                    noiseMap[coord.x, coord.y] = SmoothSection(coord.x, coord.y, squareRange, noiseMap, maxVal);
            }
        }
        return noiseMap;
    }

    private static float SmoothSection(int x, int y, int squareRange, float[,] noiseMap, float maxVal)
    {
        float average = 0;
        float impactedCells = 0;
        float smoothVal = 0;
        int rectifiedRange = Mathf.FloorToInt(squareRange/2);
        try
        {
            for (int _x = x + rectifiedRange; _x >= x - rectifiedRange; _x--)
            {
                for (int _y = y + rectifiedRange; _y >= y - rectifiedRange; _y--)
                {
                    //Debug.LogFormat("X {0}, Y {1}, _x {2} _y {3}", x, y, _x, _y);
                    average += noiseMap[_x, _y];
                    impactedCells++;
                }
            }
            smoothVal = average / impactedCells;
        }
        catch (System.Exception e)
        {
            //Debug.LogFormat("Caught {0}, trying again with lower range {1}", e, squareRange - 1);
            smoothVal = SmoothSection(x, y, squareRange - 1, noiseMap, maxVal);
        }
        if (smoothVal < 0.01f){
            smoothVal = 0;
        }
        return smoothVal;
    }
}
