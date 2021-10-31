using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkAgent : TerrainAgent
{
    public Transform chunkRoot;
    public VoxelRenderer voxelRenderer;

    public override void UpdateGrid(VoxelGrid grid)
    {
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
                    newVoxelChunk.GetComponent<VoxelRenderer>().GenerateAndUpdate(grid, x * chunkSize, y * chunkSize, z * chunkSize);
                }
            }
        }
        voxelRenderer.gameObject.SetActive(false);
    }
}
