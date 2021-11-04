using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transforms top layer of dirt to grass objects only above sea level.
/// </summary>
public class GrassAgent : TerrainAgent
{
    public TerrainData terrainData;
    public int dirtType = 3;
    public int grasType = 2;
    private int seaLevel => terrainData.seaLevel;

    public override void UpdateGrid(VoxelGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = seaLevel; y < grid.Height; y++)
                {
                    int cellType = grid.GetCell(x, y, z);
                    int aboveType = grid.GetNeighbor(x, y, z, Direction.Up);
                    if (cellType == dirtType && aboveType == 0)
                        grid.SetCell(x, y, z, grasType);
                }
            }
        }
    }
}
