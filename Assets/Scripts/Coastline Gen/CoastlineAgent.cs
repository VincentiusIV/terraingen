using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int sandTypeDistVolcano = 20;
    private HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
    private int MaxTries = 25;
    private List<(Vector3, float)> volcanoPositions;

    public override void UpdateGrid(VoxelGrid grid)
    {
        if (Loader.beachColor != 0) sandTypeDistVolcano = Loader.beachColor;
        VolcanoAgent volcanoAgent = GameObject.Find("VolcanoAgent").GetComponent<VolcanoAgent>();
        volcanoPositions = volcanoAgent.GetPositions(); 
        int tokens = (int)(grid.Width * tokenScalar);
        for (int token = 0; token < tokens; token++)
        {
            Vector3Int randomPos = new Vector3Int(Random.Range(0, grid.Width), Random.Range(minBeachHeight, maxBeachHeight), Random.Range(0, grid.Depth));
            if (nearCoast(randomPos, grid) && !visited.Contains(randomPos))
            {
                float selector = Random.Range(0, 3);
                int beachType = 4;
                visited.Add(randomPos);
                DrawBeach(randomPos, grid, itterationDepth, beachType);
            }
        }
    }

    private VoxelGrid DrawBeach(Vector3Int pos, VoxelGrid grid, int depth, int type)
    {
        if (depth < 1)
        {
            return grid;
        }
        foreach (var cell in Brush(pos, grid))
        {
            if (grid.GetCell(cell) != 0 && grid.GetMaxSlope(cell, slopeRange) < maxSlope && cell.y < maxBeachHeight)
            {
                for (int y = 0; y <= cell.y; y++)
                {
                    if (DistToVolcano(pos) < sandTypeDistVolcano)
                    {
                        grid.SetCell(cell.x, y, cell.z, 5);
                    }
                    else
                    {
                        grid.SetCell(cell.x, y, cell.z, 4);
                    }
                }
            }
        }
        pos = NextPosition(pos, grid);
        if (pos.x == -1) //No location was found when trying... stop
        {
            return grid;
        }
        Debug.Log("Found adjacent");
        grid = DrawBeach(pos, grid, depth - 1, type);
        return grid;
    }

    private Vector3Int NextPosition(Vector3Int pos, VoxelGrid grid)
    {
        Vector3Int checking = pos;
        checking.x = pos.x + (int)Random.Range(-searchRadius / 2, searchRadius / 2);
        checking.z = pos.z + (int)Random.Range(-searchRadius / 2, searchRadius / 2);
        int tries = 0;
        while (!nearCoast(checking, grid) && !visited.Contains(checking))
        {
            checking.x = pos.x + (int)Random.Range(-searchRadius / 2, searchRadius / 2);
            checking.z = pos.z + (int)Random.Range(-searchRadius / 2, searchRadius / 2);
            tries++;
            if (tries > MaxTries)
            {
                return new Vector3Int(-1, -1, -1);
            }
        }
        visited.Add(checking);
        return checking;
    }

    private List<Vector3Int> Brush(Vector3Int pos, VoxelGrid grid)
    {
        int itt = Mathf.FloorToInt(searchRadius / 2) - 1;
        bool ittToggle = false;
        HashSet<Vector3Int> cells = new HashSet<Vector3Int>();
        for (int x = (int)(-searchRadius / 2); x < (int)(searchRadius / 2) + 1; x++)
        {
            for (int z = 0; z < searchRadius; z++)
            {
                if (z > itt && z < (searchRadius - itt - 1))
                {
                    int depth = grid.GetDepth(pos.x + x, pos.y, pos.z + (z - (int)(searchRadius / 2))) - 1;
                    cells.Add(new Vector3Int(pos.x + x, pos.y + depth, pos.z + (z - (int)(searchRadius / 2))));
                }
            }
            if (itt < 0)
            {
                ittToggle = true;
            }
            if (ittToggle)
            {
                itt++;
            }
            else
            {
                itt--;
            }
        }
        List<Vector3Int> selected = cells.ToList();
        return selected;
    }

    private bool nearCoast(Vector3Int pos, VoxelGrid grid)
    {
        bool foundEmpty = false;
        bool foundFilled = false;
        for (int x = (int)(-searchRadius / 2); x < (int)(searchRadius / 2) + 1; x++)
        {
            for (int z = (int)(-searchRadius / 2); z < (int)(searchRadius / 2) + 1; z++)
            {
                if (grid.GetCell(pos.x + x, pos.y, pos.z + z) == 0 && !foundEmpty && grid.InBounds(pos.x + x, pos.y, pos.z + z))
                {
                    foundEmpty = true;
                }
                if (grid.GetCell(pos.x + x, pos.y, pos.z + z) != 0 && !foundFilled && grid.InBounds(pos.x + x, pos.y, pos.z + z))
                {
                    foundFilled = true;
                }
                if (foundFilled && foundEmpty)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private float DistToVolcano(Vector3Int pos)
    {
        if (volcanoPositions.Count != 0)
        {
            float distance = float.MaxValue;
            foreach (var volcano in volcanoPositions)
            {
                Vector3 volcPos = volcano.Item1;
                float radius = volcano.Item2;

                float xSq = Mathf.Pow(Mathf.Abs(pos.x - volcPos.x), 2);
                float ySq = Mathf.Pow(Mathf.Abs(pos.y - volcPos.y), 2);
                float zSq = Mathf.Pow(Mathf.Abs(pos.z - volcPos.z), 2);

                float currentDist = Mathf.Sqrt(xSq + ySq + zSq);
                if (currentDist < distance)
                {
                    distance = currentDist;
                }
                distance = distance - radius;
            }
            return distance;
        }
        else
        {
            return float.MaxValue;
        }

    }
}