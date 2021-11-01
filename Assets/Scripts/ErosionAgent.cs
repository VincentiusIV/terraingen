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
        for (int i = 0; i < voxelMaterials.Count; i++)
        {
            VoxelMaterial material = voxelMaterials[i];
            float[,] noiseMap = Noise.GenerateNoiseMap(grid.Width, grid.Depth, noiseScale, octaves, persistance, material.roughness, grid.Width, new Vector2(transform.position.x, transform.position.z));

            for (int x = 0; x < grid.Width; x++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        int depth = grid.GetDepth(x, y, z);
                        int cellType = grid.GetCell(x, y, z);
                        float matDepth = material.depth;
                        if (i == voxelMaterials.Count - 1)
                            matDepth += (noiseMap[x, z] - 0.5f) * material.thickness;
                        matDepth = Mathf.Max(matDepth, 1);

                        if (cellType == 0 || depth < matDepth)
                        {
                            continue;
                        }

                        grid.SetCell(x, y, z, material.index);
                    }
                }
            }
        }
    }
}
