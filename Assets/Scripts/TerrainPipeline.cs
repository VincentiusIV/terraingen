using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPipeline : MonoBehaviour
{
    public VoxelGrid Grid { get; private set; }
    public TerrainAgent[] agents;

    public int size = 10000;
    public int maxHeight = 10;

    private void Awake()
    {
        GenerateTerrain();
    }

    [ContextMenu("Run")]
    public void GenerateTerrain()
    {
        Debug.Log("Running the terrain pipeline...");
        Grid = new VoxelGrid(size, maxHeight);
        foreach (var agent in agents)
        {
            Debug.LogFormat("Agent active: {0}", agent.GetType().ToString());
            agent.UpdateGrid(Grid);
        }
    }
}
