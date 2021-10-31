using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelMaterial
{
    public string name;
    public int[] indices = new int[6];

    public Vector2[] GetUVs(Texture texture, int horizontalCells, int verticalCells, int cellWidth, int cellHeight, Direction direction)
    {
        float width = texture.width, height = texture.height;
        int index = indices[(int)direction];
        float x = ((float)cellWidth * (index % horizontalCells)) / width;
        float y = 1f - (((float)cellHeight * (index / horizontalCells)) / height);
        float xleft = cellWidth / width;
        float ydown = cellHeight / height;
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(x, y);
        uvs[1] = new Vector2(x + xleft, y);
        uvs[2] = new Vector2(x + xleft, y - ydown);
        uvs[3] = new Vector2(x, y - ydown);
        return uvs;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRenderer : MonoBehaviour
{
    public float scale = 1f;
    public bool drawGizmos = false;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh
    {
        get => Application.isPlaying ? meshFilter.mesh : meshFilter.sharedMesh;
        set
        {
            if (Application.isPlaying)
                meshFilter.mesh = value;
            else
                meshFilter.sharedMesh = value;
        }
    }

    public int chunkSize = 10;
    public int textureRows = 16, textureColumns = 16;
    public Texture textureSheet;
    public VoxelMaterial[] materials;
    private int cellWidth => textureSheet.width / textureColumns;
    private int cellHeight => textureSheet.height / textureRows;

    private int x, y, z;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;
    private float adjScale;
    private VoxelGrid grid;

    private readonly static Vector3[] cubeVertices =
    {
        new Vector3(1,1,1),
        new Vector3(-1,1,1),
        new Vector3(-1,-1,1),
        new Vector3(1,-1,1),
        new Vector3(-1,1,-1),
        new Vector3(1,1,-1),
        new Vector3(1,-1,-1),
        new Vector3(-1,-1,-1)
    };

    private readonly static int[,] faceTriangles =
    {
        { 0, 1, 2, 3},
        { 5, 0, 3, 6},
        { 4, 5, 6, 7},
        { 1, 4, 7, 2},
        { 5, 4, 1, 0},
        { 3, 2, 7, 6},
    };

    [ContextMenu("GenerateDefault")]
    private void GenerateDefault()
    {
        GenerateAndUpdate(new VoxelGrid(10, 10), 0, 0, 0);
    }

    public void GenerateAndUpdate(VoxelGrid voxelGrid, int x, int y, int z)
    {
        this.grid = voxelGrid;
        this.x = x;
        this.y = y;
        this.z = z;
        meshFilter = GetComponent<MeshFilter>();
        adjScale = scale * 0.5f;
        CreateVoxelMesh(voxelGrid);
        UpdateMesh();
    }

    private void CreateVoxelMesh(VoxelGrid voxelGrid)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    int xOffset = x + this.x;
                    int yOffset = y + this.y;
                    int zOffset = z + this.z;
                    int type = voxelGrid.GetCell(xOffset, yOffset, zOffset);
                    if (type != 0)
                    {
                        CreateCube(adjScale, new Vector3(xOffset * scale + adjScale, yOffset * scale + adjScale, zOffset * scale + adjScale), voxelGrid, type);
                    }
                }
            }
        }
    }

    private void CreateCube(float scale, Vector3 cubePos, VoxelGrid voxelGrid, int type)
    {
        for (int i = 0; i < 6; i++)
        {
            if(voxelGrid.GetNeighbor(cubePos, (Direction)i) == 0)
                MakeFace((Direction)i, scale, cubePos, type);
        }
    }

    private void MakeFace(Direction dir, float scale, Vector3 facePos, int type)
    {
        vertices.AddRange(FaceVertices((int)dir, scale, facePos));
        int vertexCount = vertices.Count;
        triangles.Add(vertexCount - 4);
        triangles.Add(vertexCount - 4 + 1);
        triangles.Add(vertexCount - 4 + 2);
        triangles.Add(vertexCount - 4);
        triangles.Add(vertexCount - 4 + 2);
        triangles.Add(vertexCount - 4 + 3);

        VoxelMaterial mat = materials[type - 1];
        uvs.AddRange(mat.GetUVs(textureSheet, textureColumns, textureRows, cellWidth, cellHeight, dir));
    }

    private Vector3[] FaceVertices(int dir, float scale, Vector3 facePos)
    {
        Vector3[] faceVertices = new Vector3[4];
        for (int i = 0; i < faceVertices.Length; i++)
            faceVertices[i] = cubeVertices[faceTriangles[dir, i]] * scale + facePos;
        return faceVertices;
    }

    private void UpdateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmosSelected()
    {
        if (grid == null || !drawGizmos)
            return;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    if (grid.GetCell(x, y, z) != 0)
                        Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * scale);
                }
            }
        }
    }
}
