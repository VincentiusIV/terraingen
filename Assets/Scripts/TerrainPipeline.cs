using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPipeline : MonoBehaviour
{
    public TerrainAgent[] agents;
    public VoxelRenderer voxelRenderer;

    public int chunkSize = 10;
    public int chunkHeight = 10;

    private void Awake()
    {
        GenerateTerrain();
    }

    [ContextMenu("Run")]
    public void GenerateTerrain()
    {
        VoxelGrid grid = new VoxelGrid(chunkSize, chunkHeight, transform.position);
        foreach (var agent in agents)
        {
            agent.UpdateGrid(grid);
        }
        voxelRenderer.GenerateAndUpdate(grid);
    }
}
