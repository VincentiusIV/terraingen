using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPipeline : MonoBehaviour
{
    public TerrainAgent[] agents;
    public Transform chunkRoot;
    public VoxelRenderer voxelRenderer;

    public int size = 10000;
    public int maxHeight = 10;

    private void Awake()
    {
        GenerateTerrain();
    }

    [ContextMenu("Run")]
    public void GenerateTerrain()
    {
        VoxelGrid grid = new VoxelGrid(size, maxHeight);
        foreach (var agent in agents)
        {
            agent.UpdateGrid(grid);
        }

        int numChunkX = Mathf.CeilToInt(grid.Width / voxelRenderer.chunkSize);
        int numChunkY = Mathf.CeilToInt(grid.Height / voxelRenderer.chunkSize);
        int numChunkZ = Mathf.CeilToInt(grid.Depth / voxelRenderer.chunkSize);
        chunkRoot.DestroyChildren();
        voxelRenderer.gameObject.SetActive(true);
        int chunkSize = voxelRenderer.chunkSize;
        for (int x = 0; x < numChunkX; x++)
        {
            for (int y = 0; y < numChunkY; y++)
            {
                for (int z = 0; z < numChunkZ; z++)
                {
                    GameObject newVoxelChunk = Instantiate(voxelRenderer.gameObject, chunkRoot);
                    newVoxelChunk.GetComponent<VoxelRenderer>().GenerateAndUpdate(grid, x * chunkSize, y * chunkSize , z * chunkSize);
                }
            }
        }
        voxelRenderer.gameObject.SetActive(false);
    }
}
