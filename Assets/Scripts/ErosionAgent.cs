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
        SetMaterialLayers(grid);
    }
    
    private void SetMaterialLayers(VoxelGrid grid)
    {
        List<VoxelMaterial> voxelMaterials = new List<VoxelMaterial>();
        voxelMaterials.AddRange(terrainData.materials);
        voxelMaterials.OrderByDescending(m => m.depth);
        voxelMaterials.RemoveAll(m => m.materialType == MaterialType.Ignored);
        List<float[,]> noiseMaps = new List<float[,]>();
        for (int i = 0; i < voxelMaterials.Count; i++)
        {
            VoxelMaterial material = voxelMaterials[i];
            float[,] noiseMap = Noise.GenerateNoiseMap(grid.Width, grid.Depth, noiseScale, octaves, persistance, material.roughness, grid.Width, new Vector2(transform.position.x, transform.position.z));
            noiseMaps.Add(noiseMap);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        int cellType = grid.GetCell(x, y, z);
                        if (cellType == 0)
                            continue;
                        int depth = grid.GetDepth(x, y, z);
                        float materialDepth = material.depth;
                        if(depth >= materialDepth)
                        {
                            grid.SetCell(x, y, z, material.index);
                        }
                    }
                }
            }
        }
    }
}
