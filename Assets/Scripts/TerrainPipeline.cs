using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
public class TerrainPipeline : MonoBehaviour
{
    public VoxelGrid Grid { get; private set; }
    public TerrainAgent[] agents;

    public TerrainData terrainData;
    public int size = Loader.size;
    public int maxHeight = Loader.height;

    private void Awake() {
        size = Loader.size;
        GenerateTerrain();
    }

    [ContextMenu("Run")]
    public void GenerateTerrain()
    {
        UnityEngine.Debug.LogFormat("{0} - {1} - {2} - {3} - {4} ", Loader.size, Loader.height, Loader.noiseOffset, Loader.volcs, Loader.caves);
        UnityEngine.Debug.Log("Running the terrain pipeline...");
        Stopwatch totalWatch = new Stopwatch();
        totalWatch.Start();
        Stopwatch agentWatch = new Stopwatch();
        Grid = new VoxelGrid(size, maxHeight);
        foreach (var agent in agents)
        {
            agent.terrainData = terrainData;

            agentWatch.Start();
            UnityEngine.Debug.LogFormat("Agent active: {0}", agent.GetType().ToString());
            agent.UpdateGrid(Grid);
            Grid.UpdateDepthsAndHeights();
            agentWatch.Stop();
            PrintTimeStamp(agentWatch, agent.GetType().ToString());
            agentWatch.Reset();
        }
        totalWatch.Stop();
        PrintTimeStamp(totalWatch, "Terrain Pipeline");
    }

    private static void PrintTimeStamp(Stopwatch totalWatch, string prefix)
    {
        TimeSpan ts = totalWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.LogFormat("{0} finished after t={1}s", prefix, elapsedTime);
    }
}
