using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionAgent : TerrainAgent
{
    public TerrainData terrainData;

    public float noiseScale;
    public int octaves;
    public float persistance;
    public float falloff;

    public override void UpdateGrid(VoxelGrid grid)
    {
        SetMaterialsByDepth(grid);
        SetMaterialLayers(grid);
    }

    private static void SetMaterialsByDepth(VoxelGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = grid.Height; y >= 0; y--)
                {
                    if(grid.GetCell(x, y, z) != 0)
                        grid.SetCell(x, y, z, 1);
                }
            }
        }
    }
    
    private void SetMaterialLayers(VoxelGrid grid)
    {
        List<VoxelMaterial> voxelMaterials = new List<VoxelMaterial>();
        voxelMaterials.AddRange(terrainData.materials);
        voxelMaterials.OrderByDescending(m => m.depth);
        foreach (var material in voxelMaterials)
        {
            if (material.materialType == MaterialType.Ignored)
                continue;
            float minNoise = float.MaxValue, maxNoise = float.MinValue;

            float[,] noiseMap = Noise.GenerateNoiseMap(grid.Width, grid.Depth, noiseScale, octaves, persistance, material.roughness, falloff, new Vector2(transform.position.x, transform.position.z));
            
            for (int x = 0; x < grid.Width; x++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        int depth = grid.GetDepth(x, y, z);
                        int cellType = grid.GetCell(x, y, z);
                        if (cellType == 0 || depth < material.depth)
                            continue;

                        if(material.index != voxelMaterials.Count)
                        {
                            float noise = noiseMap[x, z];
                            if (noise > maxNoise)
                                maxNoise = noise;
                            if (noise < minNoise)
                                minNoise = noise;
                        }
                        grid.SetCell(x, y, z, material.index);
                    }
                }
            }
            Debug.LogFormat("For layer {0}, noise range is {1} - {2}", material.name, minNoise, maxNoise);
        }
    }
}
