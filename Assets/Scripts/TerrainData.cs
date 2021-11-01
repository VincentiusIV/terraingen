using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Terrain/Data")]
public class TerrainData : ScriptableObject
{
    private int cellWidth => textureSheet.width / textureColumns;
    private int cellHeight => textureSheet.height / textureRows;

    public int textureRows = 16, textureColumns = 16;
    public Texture textureSheet;
    public VoxelMaterial[] materials;

    public VoxelMaterial GetMaterial(int type)
    {
        return materials[type - 1];
    }
}
