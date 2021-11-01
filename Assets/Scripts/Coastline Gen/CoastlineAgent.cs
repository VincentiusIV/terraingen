using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastlineAgent : TerrainAgent
{
    public int sxSearch = 5;
    public int sySearch = 5;
    public int szSearch = 5;
    public float maxSlope = 1.7f;
    public override void UpdateGrid(VoxelGrid grid)
    {
        Debug.Log("CoastlineAgent working...");
        for (int x = 0; x < grid.Width; x += sxSearch)
        {
            for (int y = 0; y < 5; y += sySearch)
            {
                for (int z = 0; z < grid.Depth; z += szSearch)
                {
                    List<Vector3Int> emptyCells = GetNearbyEmpty(new Vector3Int(x, y, z), grid);
                    List<Vector3Int> surfaceCells = GetNearbySurface(new Vector3Int(x, y, z), grid);

                    if (emptyCells.Count != 0)
                    {
                        //Found beachfront
                        foreach (var item in surfaceCells)
                        {
                            if (grid.GetHeight(item.x, item.y, item.z) < 3)
                            {
                                if (grid.GetMaxSlope(item.x, item.y, item.z, 5) < maxSlope)
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
                        }
                    }
                }
            }

        }

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