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

    int[,,] cells = new int[,,] { { 
          { 1, 0, 0}, { 1, 1 ,0}, { 1, 0, 0 } }, 
        { { 1, 0, 0 }, { 1, 1, 0 }, { 1, 0, 0 } }, 
        { { 1, 0, 0 }, { 1, 1, 0 }, { 1, 0, 0} } };

    public int GetCell(int x, int y, int z)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            return 0;
        return cells[x, z, y];
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
        return GetNeighbor(Vector3Int.CeilToInt(position), dir);
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
