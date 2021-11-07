using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainAgent : MonoBehaviour
{
    public TerrainData terrainData { get; set; }

    public abstract void UpdateGrid(VoxelGrid grid);
}
