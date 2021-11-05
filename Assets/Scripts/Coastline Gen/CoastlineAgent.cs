using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastlineAgent : TerrainAgent
{
    [Header("Token amount = gridWidth * tokenScalar")]
    public float tokenScalar = 1;
    public int minBeachHeight = 4;
    public int maxBeachHeight = 8;
    public int searchRadius = 7;
    private int savedSR;
    public int itterationDepth = 6;
    public float maxSlope = 1.5f;
    public int slopeRange = 3;

    public override void UpdateGrid(VoxelGrid grid)
    {

        int tokens = (int)(grid.Width * tokenScalar);
        savedSR = searchRadius;
        Debug.Log("Beach Agent Working... (Pounding Sand)");
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = 0; y < minBeachHeight; y++)
                {
                    grid.SetCell(x, y, z, 4);
                }
            }
        }
        minBeachHeight++;
        for (int token = 0; token < tokens; token++)
        {
            Vector2 randXZ = Random.insideUnitCircle;
            int type = (int)Mathf.Round(randXZ.x);
            if (type == 1) type = 4;
            if (type == 0) type = 5;
            Vector3Int position = new Vector3Int((int)(randXZ.x * grid.Width), minBeachHeight, (int)(randXZ.y * grid.Depth));
            grid = MakeBeach(position, grid, type);
            //Debug.Log(position);
            searchRadius = savedSR; //Reset decrementing SR
        }

    }

    private VoxelGrid MakeBeach(Vector3Int position, VoxelGrid grid, int type)
    {
        List<Vector3Int> beachLine = ScanCells(position, grid, 0);
        List<Vector3Int> surfaceCells = ScanCells(position, grid, 1);
        if (beachLine.Count != 0 && surfaceCells.Count != 0)
        {
            foreach (var cell in surfaceCells)
            {
                if (grid.GetMaxSlope(cell, slopeRange) < maxSlope && cell.y < maxBeachHeight)
                {
                    grid.SetCell(cell, type);
                }
            }
            grid = ExpandBeach(position, grid, itterationDepth, type);
            //grid = ConnectBeach(grid);
        }
        return grid;
    }

    private VoxelGrid ExpandBeach(Vector3Int position, VoxelGrid grid, int depth, int type)
    {
        if(depth < 1)
        {
            return grid;
        }

        for (int i = 0; i < 4; i++)
        {
            if(FindNext(position, grid, i).x != -1)
            {
                position = FindNext(position, grid, i);
                break;
            }
            else
            {
                return grid; //No suitable land nearby
            }
        }
        /*
        if((int)Random.Range(0,8) < 1 && position.y < maxBeachHeight) //12.5%
        {
            position.y++;
        }
        if ((int)Random.Range(0, 8) < 1 && position.y > minBeachHeight) //12.5%
        {
            position.y--;
        }
        */
        List<Vector3Int> beachLine = ScanCells(position, grid, 2);
        List<Vector3Int> surfaceCells = ScanCells(position, grid, 1);
        if (beachLine.Count != 0 && surfaceCells.Count != 0)
        {
            foreach (var cell in surfaceCells)
            {
                if (grid.GetMaxSlope(cell, slopeRange) < maxSlope && cell.y < maxBeachHeight)
                {
                    grid.SetCell(cell, type);
                }
            }
        }
        if (searchRadius > 3 && (int)Random.Range(0, 3) < 1)
        {
            searchRadius--;
        }
        grid = ExpandBeach(position, grid, depth - 1, type);

        return grid;
    }

    private Vector3Int FindNext(Vector3Int position, VoxelGrid grid, int dir)
    {
        switch (dir)
        {
            case 0:
                position.x = position.x + searchRadius;
                break;
            case 1:
                position.x = position.x - searchRadius;
                break;
            case 2:
                position.z = position.z + searchRadius;
                break;
            case 3:
                position.z = position.z - searchRadius;
                break;
        }
        List<Vector3Int> beachLine = ScanCells(position, grid, 2);
        List<Vector3Int> surfaceCells = ScanCells(position, grid, 1);
        if (beachLine.Count != 0 && surfaceCells.Count != 0)
        {
            return position;
        }
        return new Vector3Int(-1,-1,-1);
    }

    private List<Vector3Int> ScanCells(Vector3Int position, VoxelGrid grid, int mode) 
        //0: Get empty cells at y = minBeachHeight. 1: get Surface in a xyz radius 
        //2: get empty at specified Y
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        int search = Mathf.FloorToInt(searchRadius / 2);
        for (int x = -search; x < search + 1; x++)
        {
            for (int z = -search; z < search + 1; z++)
            {
                if (mode == 0)
                {
                    if (grid.GetCell(position.x + x, minBeachHeight, position.z + z) == 0 && grid.InBounds(position.x + x, minBeachHeight, position.z + z))
                    {
                        cells.Add(new Vector3Int(position.x + x, minBeachHeight, position.z + z));
                    }
                }else if(mode == 2)
                {
                    if (grid.GetCell(position.x + x, position.y, position.z + z) == 0 && grid.InBounds(position.x + x, position.y, position.z + z))
                    {
                        cells.Add(new Vector3Int(position.x + x, position.y, position.z + z));
                    }
                }
                else if (mode == 1)
                {
                    for (int y = -searchRadius; y < searchRadius; y++)
                    {
                        if (grid.GetCell(position.x + x, y, position.z + z) != 0 && grid.InBounds(position.x + x, minBeachHeight, position.z + z))
                        {
                            cells.Add(new Vector3Int(position.x + x, y, position.z + z));
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        return cells;
    }
}