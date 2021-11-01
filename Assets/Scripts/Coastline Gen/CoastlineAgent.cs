using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastlineAgent : TerrainAgent
{
    public int tokens = 100;
    public int searchDepth = 5;
    private int currentSearchDepth;
    public int sxSearch = 5;
    public int sySearch = 5;
    public int szSearch = 5;
    public float maxSlope = 1.7f;
    public int maxBeachHeight = 4;
    public override void UpdateGrid(VoxelGrid grid)
    {
        Debug.Log("CoastlineAgent working...");
        //token based agent picking random positions
        for (int token = 0; token < tokens; token++)
        {
            Debug.LogFormat("Used {0} out of {1} tokens", token, tokens);
            Vector3Int position = new Vector3Int((int)Random.Range(0, grid.Width), 2, (int)Random.Range(0, grid.Depth));
            DrawBeaches(position, grid, searchDepth);
        }
    }

    private void DrawBeaches(Vector3Int position, VoxelGrid grid, int depth)
    {

        List<Vector3Int> emptyCells = GetNearbyEmpty(position, grid);
        List<Vector3Int> surfaceCells = GetNearbySurface(position, grid);

        if (emptyCells.Count != 0 && depth > 0)
        {
            //Found beachfront
            foreach (var item in surfaceCells)
            {
                if (grid.GetHeight(item.x, item.y, item.z) < 6 && (grid.GetMaxSlope(item.x, item.y, item.z, sxSearch) < maxSlope))
                {
                    for (int i = item.y; i >= 0; i--)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            for (int k = -1; k < 2; k++)
                            {
                                if (grid.GetCell(item.x + j, i, item.z + k) != 0)
                                    grid.SetCell(item.x + j, i, item.z, 1);
                            }
                        }
                    }

                }
            }
            depth -= 1;
            DrawBeaches(RandomNeighbour(position), grid, depth);
        }
        
        else if (depth > 0)
        {
            depth = depth - 1;
            DrawBeaches(RandomNeighbour(position), grid, depth);
        }
    }

    private Vector3Int RandomNeighbour(Vector3Int position)
    {
        int selector = (int)Random.Range(0, 4);
        switch (selector)
        {
            case 0:
                position.x += sxSearch;
                Debug.Log("Went Left");
                break;
            case 1:
                position.x -= sxSearch;
                Debug.Log("Went Right");
                break;
            case 2:
                position.z += szSearch;
                Debug.Log("Went Inward");
                break;
            case 3:
                position.z -= szSearch;
                Debug.Log("Went Outward");
                break;
            default:
                break;
        }
        return position;
    }

    private List<Vector3Int> GetNearbyEmpty(Vector3Int position, VoxelGrid grid)
    {
        List<Vector3Int> emptyCells = new List<Vector3Int>();
        //makes sure the centerblock is the centerblock and no fractional coords are used
        int xSearch = Mathf.FloorToInt(sxSearch / 2);
        int ySearch = Mathf.FloorToInt(sySearch / 2);
        int zSearch = Mathf.FloorToInt(szSearch / 2);
        if (position.x - xSearch < 0) position.x = xSearch;
        else if (position.x + xSearch > grid.Width) position.x = grid.Width - xSearch;
        if (position.y - ySearch < 0) position.y = ySearch;
        else if (position.y + ySearch > grid.Width) position.y = grid.Height - ySearch;
        if (position.z - zSearch < 0) position.z = zSearch;
        else if (position.z + zSearch > grid.Depth) position.z = grid.Depth - zSearch;

        for (int i = -xSearch; i <= xSearch; i++)
        {
            for (int j = -ySearch; j <= ySearch; j++)
            {
                for (int k = -zSearch; k <= zSearch; k++)
                {
                    int currentSearchingX = (int)position.x - i;
                    int currentSearchingY = (int)position.y - j;
                    int currentSearchingZ = (int)position.z - k;
                    int cellType = grid.GetCell(currentSearchingX, currentSearchingY, currentSearchingZ);
                    if (cellType == 0)
                    {
                        emptyCells.Add(new Vector3Int(currentSearchingX, currentSearchingY, currentSearchingZ));
                    }
                }
            }
        }

        return emptyCells;
    }
    private List<Vector3Int> GetNearbySurface(Vector3Int position, VoxelGrid grid)
    {
        List<Vector3Int> surfaceCells = new List<Vector3Int>();
        //makes sure the centerblock is the centerblock and no fractional coords are used
        int xSearch = Mathf.FloorToInt(sxSearch / 2);
        int ySearch = Mathf.FloorToInt(sySearch / 2);
        int zSearch = Mathf.FloorToInt(szSearch / 2);
        if (position.x - xSearch < 0) position.x = xSearch;
        else if (position.x + xSearch > grid.Width) position.x = grid.Width - xSearch;
        if (position.y - ySearch < 0) position.y = ySearch;
        else if (position.y + ySearch > grid.Width) position.y = grid.Height - ySearch;
        if (position.z - zSearch < 0) position.z = zSearch;
        else if (position.z + zSearch > grid.Depth) position.z = grid.Depth - zSearch;

        for (int i = -xSearch; i <= xSearch; i++)
        {
            for (int j = -ySearch; j <= ySearch; j++)
            {
                for (int k = -zSearch; k <= zSearch; k++)
                {
                    int currentSearchingX = (int)position.x - i;
                    int currentSearchingY = (int)position.y - j;
                    int currentSearchingZ = (int)position.z - k;
                    int cellDepth = grid.GetDepth(currentSearchingX, currentSearchingY, currentSearchingZ);
                    if (cellDepth == 1)
                    {
                        surfaceCells.Add(new Vector3Int(currentSearchingX, currentSearchingY, currentSearchingZ));
                    }
                }
            }
        }

        return surfaceCells;
    }
}