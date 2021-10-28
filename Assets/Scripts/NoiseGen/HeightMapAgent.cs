using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes voxel grid with a simple height map
/// </summary>
public class HeightMapAgent : TerrainAgent
{
    public MapGenerator mapGenerator;

    public override void UpdateGrid(VoxelGrid grid)
    {
        float[,] noiseMap = mapGenerator.Generate();
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                float height = noiseMap[x, z] * grid.Height;
                for (int y = 0; y < grid.Height; y++)
                {
                    if(y < height)
                    {
                        grid.SetCell(x, y, z, 1);
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        if (mapGenerator == null)
            mapGenerator = GetComponentInChildren<MapGenerator>();
    }
}
