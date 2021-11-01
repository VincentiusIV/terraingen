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
            Vector3Int position = new Vector3Int((int)Random.Range(0, grid.Width/5)*5, 2, (int)Random.Range(0, grid.Depth/5)*5);
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
            FinishBeach(position, grid, new Stack<Vector3Int>(), depth * 2);
        }
        
        else if (depth > 0)
        {
            depth = depth - 1;
            DrawBeaches(RandomNeighbour(position), grid, depth);
        }
    }

    private void FinishBeach(Vector3Int position, VoxelGrid grid, Stack<Vector3Int> stack, int depth)
    {
        List<Vector3Int> emptyCells = GetNearbyEmpty(position, grid);
        List<Vector3Int> surfaceCells = GetNearbySurface(position, grid);
        if (emptyCells.Count != 0 && depth > 0)
        {
            foreach (var item in surfaceCells)
            {
                stack.Push(item);
            }
            depth = depth - 1;
            FinishBeach(RandomNeighbour(position), grid, stack, depth);
        }
        else
        {
            while(stack.Count != 0)
            {
                Vector3Int voxel = stack.Pop();
                if (grid.GetHeight(voxel.x, voxel.y, voxel.z) < 6 && (grid.GetMaxSlope(voxel.x, voxel.y, voxel.z, sxSearch) < maxSlope))
                {
                    for (int i = voxel.y; i >= 0; i--)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            for (int k = -1; k < 2; k++)
                            {
                                if (grid.GetCell(voxel.x + j, i, voxel.z + k) != 0)
                                    grid.SetCell(voxel.x + j, i, voxel.z, 1);
                            }
                        }
                    }

                }
            }
        }
    }

    private Vector3Int getNeigbour(Vector3Int position, int selector)
    {
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