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
        InitializeLayers(grid, voxelMaterials);

        List<(int, int)>[,] layerRepresentation = new List<(int, int)>[grid.Width, grid.Depth];
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                int currentCell = grid.GetCell(x, 0, z);
                int counter = 0;
                if (layerRepresentation[x, z] == null)
                    layerRepresentation[x, z] = new List<(int, int)>();
                for (int y = 0; y < grid.Height; y++)
                {
                    int cell = grid.GetCell(x, y, z);
                    if (cell == 0)
                    {
                        layerRepresentation[x, z].Add((currentCell, counter));
                        break;
                    }
                    else if (cell == currentCell)
                        ++counter;
                    else if(currentCell != cell)
                    {
                        layerRepresentation[x, z].Add((currentCell, counter));
                        currentCell = cell;
                        counter = 1;
                    }
                }
            }
        }

        Debug.Log("Layer rep for (19, 0)=");
        List<(int, int)> layerReps = layerRepresentation[0, 10];
        for (int i = 0; i < layerReps.Count; i++)
        {
            Debug.LogFormat("{0}: {1}x{2}", i, layerReps[i].Item2, terrainData.GetMaterial(layerReps[i].Item1).name);
        }
    }

    private void InitializeLayers(VoxelGrid grid, List<VoxelMaterial> voxelMaterials)
    {
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
                        int cellType = grid.GetCell(x, y, z);
                        if (cellType == 0)
                            continue;
                        int depth = grid.GetDepth(x, y, z);
                        float materialDepth = material.depth + noiseMap[x, z];
                        if (depth >= materialDepth)
                        {
                            grid.SetCell(x, y, z, material.index);
                        }
                    }
                }
            }
        }
    }
}
