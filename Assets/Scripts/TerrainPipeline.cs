using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

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
        UnityEngine.Debug.Log("Running the terrain pipeline...");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Grid = new VoxelGrid(size, maxHeight);
        foreach (var agent in agents)
        {
            UnityEngine.Debug.LogFormat("Agent active: {0}", agent.GetType().ToString());
            agent.UpdateGrid(Grid);
            Grid.UpdateDepthsAndHeights();
        }
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.LogFormat("Pipeline finished after t={0}s", elapsedTime);
    }
}
