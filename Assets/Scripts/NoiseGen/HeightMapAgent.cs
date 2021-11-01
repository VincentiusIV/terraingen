using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes voxel grid with a simple height map
/// </summary>
public class HeightMapAgent : TerrainAgent
{
    public MapGenerator mapGenerator;
    public int maxHeight = 10;
    public int minHeight = 0;

    public override void UpdateGrid(VoxelGrid grid)
    {
        Debug.Log("HeightMapAgent working...");
        InitializeGridFromNoiseMap(grid);
        SetMaterialsByDepth(grid);
    }

    private void InitializeGridFromNoiseMap(VoxelGrid grid)
    {
        mapGenerator.width = grid.Width;
        mapGenerator.height = grid.Depth;
        float[,] noiseMap = mapGenerator.Generate();
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                float avgHeight = GetAverage(x, z, noiseMap, grid.Width, grid.Depth);
                float height = Mathf.Lerp(minHeight, maxHeight, avgHeight);
                for (int y = 0; y < grid.Height; y++)
                {
                    if (y < height)
                    {
                        grid.SetCell(x, y, z, 1);
                    }
                }
            }
        }
        grid.UpdateDepths();
    }

    private static void SetMaterialsByDepth(VoxelGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = grid.Height; y >= 0; y--)
                {
                    int depth = grid.GetDepth(x, y, z);

                    if (depth == 1)
                        grid.SetCell(x, y, z, 1);
                    else if (depth > 1 && depth <= 4)
                        grid.SetCell(x, y, z, 2);
                    else if (depth > 4)
                        grid.SetCell(x, y, z, 3);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (mapGenerator == null)
            mapGenerator = GetComponentInChildren<MapGenerator>();
    }

    private float GetAverage(int x, int z, float[,] noisemap, int width, int depth)
    {
        if(x >= 0 && x < width && z >= 0 && z < depth)
        {
            try
            {
                float total = 0f;
                total += noisemap[x, z] + noisemap[x - 1, z] + noisemap[x + 1, z] + noisemap[x, z - 1] + noisemap[x, z + 1] + noisemap[x - 1, z + 1] + noisemap[x - 1, z - 1] + noisemap[x + 1, z + 1] + noisemap[x + 1, z - 1];
                total = total / 9;
                return total;
            }catch(System.IndexOutOfRangeException e)
            {
                return noisemap[x, z];
            }
            
        }
        else
        {
            return noisemap[x, z];
        }
    }
}
