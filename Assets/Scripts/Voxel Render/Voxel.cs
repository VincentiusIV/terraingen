using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
    Up = 4,
    Down = 5
}


public class VoxelGrid
{
    public int Width => cells.GetLength(0);
    public int Depth => cells.GetLength(1);
    public int Height => cells.GetLength(2);
    
    private int[,,] cells;
    private int[,,] depths;

    public VoxelGrid(int size, int height)
    {
        cells = new int[size, size, height];
        depths = new int[size, size, height];
    }

    public int GetCell(int x, int y, int z)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            return 0;
        return cells[x, z, y];
    }

    public void SetCell(int x, int y, int z, int type)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            return;
        cells[x, z, y] = type;
    }

    public int GetDepth(int x, int y, int z)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            return 0;
        return depths[x, z, y];
    }

    public void SetDepth(int x, int y, int z, int depth)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            return;
        depths[x, z, y] = depth;
    }

    public void UpdateDepths()
    {
        Debug.Log("Voxel Grid: Updating Depths...");
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                int depth = 0;
                for (int y = Height; y >= 0; y--)
                {
                    if (GetCell(x, y, z) == 0)
                        depth = 0;
                    else
                        ++depth;
                    SetDepth(x, y, z, depth);
                }
            }
        }
        Debug.Log("Voxel Grid: Depths update complete.");
    }

    public int GetCell(Vector3 position)
    {
        return GetCell(Vector3Int.CeilToInt(position));
    }

    public int GetCell(Vector3Int position)
    {
        return GetCell(position.x, position.y, position.z);
    }
    
    public int GetNeighbor(Vector3 position, Direction dir)
    {
        return GetNeighbor(Vector3Int.FloorToInt(position), dir);
    }
    
    public int GetNeighbor(Vector3Int position, Direction dir)
    {
        return GetNeighbor(position.x, position.y, position.z, dir);
    }
    
    public int GetNeighbor(int x, int y, int z, Direction dir)
    {
        Vector3Int offset = offsets[(int)dir];
        Vector3Int neighbor = new Vector3Int(x + offset.x, y + offset.y, z + offset.z);
        return GetCell(neighbor.x, neighbor.y, neighbor.z);
    }

    Vector3Int[] offsets =
    {
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };
}
