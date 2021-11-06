using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainLayer
{
    public int materialIndex;
    public float topY;
    public float amount;

    public TerrainLayer(int materialIndex, float topY, float amount)
    {
        this.materialIndex = materialIndex;
        this.topY = topY;
        this.amount = amount;
    }
}

public class ErosionAgent : TerrainAgent
{
    public TerrainData terrainData;
    
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float falloff;
    public int iterations = 3;
    public float c = 0.1f;

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

        List<TerrainLayer>[,] layerRepresentation = CreateLayerRepresentations(grid);
        for (int iteration = 0; iteration < iterations; iteration++)
        {

            for (int i = 0; i < voxelMaterials.Count; i++)
            {
                VoxelMaterial material = voxelMaterials[i];
                float tanRepose = Mathf.Tan(material.angleOfRepose * Mathf.Deg2Rad);
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int z = 0; z < grid.Depth; z++)
                    {
                        float slope = FindMaxSlope(x, z, material.index, layerRepresentation, grid.Width, grid.Depth);
                        if (slope < tanRepose)
                            continue;
                        float hk = FindSlopeSum(x, z, material.index, layerRepresentation, grid.Width, grid.Depth);
                        float height = GetHeight(x, z, material.index, layerRepresentation);

                        for (int ni = -1; ni < 2; ni++)
                        {
                            for (int nj = -1; nj < 2; nj++)
                            {
                                if (ni == 0 && nj == 0)
                                    continue;
                                int nx = x + ni;
                                int nz = z + nj;
                                if (nx < 0 || nx >= grid.Width || nz < 0 || nz >= grid.Depth)
                                    continue;
                                float neighborHeight = GetHeight(nx, nz, material.index, layerRepresentation);
                                float deltaHi = Mathf.Abs(height - neighborHeight);
                                float mi = c * deltaHi / hk;
                                if (neighborHeight < height)
                                    MoveMass(x, z, nx, nz, mi, material.index, ref layerRepresentation);
                                else
                                    MoveMass(nx, nz, x, z, mi, material.index, ref layerRepresentation);
                            }
                        }
                    }
                }
            }

            SortLayers(ref layerRepresentation, terrainData);
        }
        
        LayersToVoxels(layerRepresentation, grid);
    }

    public static List<TerrainLayer>[,] CreateLayerRepresentations(VoxelGrid grid)
    {
        List<TerrainLayer>[,] layerRepresentation = new List<TerrainLayer>[grid.Width, grid.Depth];
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                int currentCell = grid.GetCell(x, 0, z);
                int counter = 0;
                if (layerRepresentation[x, z] == null)
                    layerRepresentation[x, z] = new List<TerrainLayer>();
                for (int y = 0; y < grid.Height; y++)
                {
                    int cell = grid.GetCell(x, y, z);
                    if (cell == 0)
                    {
                        layerRepresentation[x, z].Add(new TerrainLayer(currentCell, y, counter));
                        break;
                    }
                    else if (cell == currentCell)
                    {
                        ++counter;
                    }
                    else if (currentCell != cell)
                    {
                        layerRepresentation[x, z].Add(new TerrainLayer(currentCell, y, counter));
                        currentCell = cell;
                        counter = 1;
                    }
                }
            }
        }

        return layerRepresentation;
    }

    public static void SortLayers(ref List<TerrainLayer>[,] layerRepresentation, TerrainData terrainData)
    {
        for (int x = 0; x < layerRepresentation.GetLength(0); x++)
        {
            for (int z = 0; z < layerRepresentation.GetLength(1); z++)
            {
                layerRepresentation[x, z].OrderBy(l => terrainData.GetMaterial(l.materialIndex).depth);
            }
        }
    }

    public static void LayersToVoxels(List<TerrainLayer>[,] layerRepresentation, VoxelGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                List<TerrainLayer> layers = layerRepresentation[x, z];
                int currentLayerIdx = 0;
                int amountCounter = 0;
                for (int y = 0; y < grid.Height; y++)
                {
                    if (currentLayerIdx >= layers.Count)
                    {
                        grid.SetCell(x, y, z, 0);
                        continue;
                    }

                    if (amountCounter >= Mathf.FloorToInt(layers[currentLayerIdx].amount))
                    {
                        ++currentLayerIdx;
                        amountCounter = 0;
                        if (currentLayerIdx >= layers.Count)
                        {
                            grid.SetCell(x, y, z, 0);
                            continue;
                        }
                    }

                    if (Mathf.FloorToInt(layers[currentLayerIdx].amount) > 0)
                    {
                        ++amountCounter;
                        grid.SetCell(x, y, z, layers[currentLayerIdx].materialIndex);
                    }
                   
                }
            }
        }
    }

    private void MoveMass(int xFrom, int zFrom, int xTo, int zTo, float amount, int materialIndex, ref List<TerrainLayer>[,] layerRepresentation)
    {
        if (amount == 0)
            return;
        for (int i = 0; i < layerRepresentation[xFrom, zFrom].Count; i++)
        {
            TerrainLayer fromLayer = layerRepresentation[xFrom, zFrom][i];
            
            if (fromLayer.materialIndex == materialIndex)
            {
                fromLayer.amount -= amount;
                fromLayer.amount = Mathf.Max(0, fromLayer.amount);
                layerRepresentation[xFrom, zFrom][i] = fromLayer;
                bool foundTarget = false;
                for (int j = 0; j < layerRepresentation[xTo, zTo].Count; j++)
                {
                    TerrainLayer toLayer = layerRepresentation[xTo, zTo][j];
                    if(toLayer.materialIndex == materialIndex)
                    {
                        foundTarget = true;
                        toLayer.amount += amount;
                        toLayer.amount = Mathf.Max(0, toLayer.amount);
                        layerRepresentation[xTo, zTo][j] = toLayer; 
                        break;
                    }
                }
                if (!foundTarget)
                    layerRepresentation[xTo, zTo].Add(new TerrainLayer(materialIndex, 0, amount));

                break;
            }
        }
        float yTop = 0;
        List<TerrainLayer> fromLayers = layerRepresentation[xFrom, zFrom];
        for (int i = 0; i < fromLayers.Count; i++)
        {
            TerrainLayer layer = fromLayers[i];
            yTop += layer.amount;
            layer.topY = yTop;
            layerRepresentation[xFrom, zFrom][i] = layer;
        }
        yTop = 0;
        List<TerrainLayer> toLayers = layerRepresentation[xTo, zTo];
        for (int i = 0; i < toLayers.Count; i++)
        {
            TerrainLayer layer = toLayers[i];
            yTop += layer.amount;
            layer.topY = yTop;
            layerRepresentation[xTo, zTo][i] = layer;
        }
    }

    private float FindSlopeSum(int x, int z, int materialIndex, List<TerrainLayer>[,] layerRepresentation, int width, int depth)
    {
        float height = GetHeight(x, z, materialIndex, layerRepresentation);
        float slopeSum = 0f;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int checkX = x + i;
                int checkZ = z + j;
                if (checkX < 0 || checkX >= width || checkZ < 0 || checkZ >= depth)
                    continue;
                float neighborHeight = GetHeight(checkX, checkZ, materialIndex, layerRepresentation);
                float slope = Mathf.Abs(height - neighborHeight);
                slopeSum += slope;
            }
        }
        return slopeSum;
    }

    private float FindMaxSlope(int x, int z, int materialIndex, List<TerrainLayer>[,] layerRepresentation, int width, int depth)
    {
        float height = GetHeight(x, z, materialIndex, layerRepresentation);
        float greatestSlope = float.MinValue;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int checkX = x + i;
                int checkZ = z + j;
                if (checkX < 0 || checkX >= width || checkZ < 0 || checkZ >= depth)
                    continue;
                float neighborHeight = GetHeight(checkX, checkZ, materialIndex, layerRepresentation);
                float slope = Mathf.Abs(height - neighborHeight);
                if (slope > greatestSlope)
                    greatestSlope = slope;
            }
        }
        return greatestSlope;
    }

    private (int, int) GetLowestNeighbor(int x, int z, int materialIndex, List<TerrainLayer>[,] layerRepresentation, int width, int depth)
    {
        float height = GetHeight(x, z, materialIndex, layerRepresentation);
        float lowestHeight = height;
        int lowestX = x, lowestZ = z;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int checkX = x + i;
                int checkZ = z + j;
                if (checkX < 0 || checkX >= width || checkZ < 0 || checkZ >= depth)
                    continue;
                float neighborHeight = GetHeight(checkX, checkZ, materialIndex, layerRepresentation);
                if(neighborHeight <= lowestHeight)
                {
                    lowestHeight = neighborHeight;
                    lowestX = x;
                    lowestZ = z;
                }
            }
        }
        return (lowestX, lowestZ);
    }

    private float GetHeight(int x, int z, int materialIndex, List<TerrainLayer>[,] layerRepresentation)
    {
        foreach (var layer in layerRepresentation[x, z])
        {
            if (layer.materialIndex == materialIndex)
                return layer.topY;
        }
        return 0f;
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
