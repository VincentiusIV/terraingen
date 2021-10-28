using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRenderer : MonoBehaviour
{
    public float scale = 1f;
    public bool drawGizmos = false;

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


    private List<Vector3> vertices;
    private List<int> triangles;
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
        GenerateAndUpdate(new VoxelGrid(10, 10, transform.position));
    }

    public void GenerateAndUpdate(VoxelGrid voxelGrid)
    {
        this.grid = voxelGrid;
        meshFilter = GetComponent<MeshFilter>();
        adjScale = scale * 0.5f;
        CreateVoxelMesh(voxelGrid);
        UpdateMesh();
    }

    private void CreateVoxelMesh(VoxelGrid voxelGrid)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (int x = 0; x < voxelGrid.Width; x++)
        {
            for (int y = 0; y < voxelGrid.Height; y++)
            {
                for (int z = 0; z < voxelGrid.Depth; z++)
                {
                    if (voxelGrid.GetCell(x, y, z) != 0)
                    {
                        CreateCube(adjScale, new Vector3(x * scale + adjScale, y * scale + adjScale, z * scale + adjScale), voxelGrid);
                    }
                }
            }
        }
    }

    private void CreateCube(float scale, Vector3 cubePos, VoxelGrid voxelGrid)
    {
        for (int i = 0; i < 6; i++)
        {
            if(voxelGrid.GetNeighbor(cubePos, (Direction)i) == 0)
                MakeFace((Direction)i, scale, cubePos);
        }
    }

    private void MakeFace(Direction dir, float scale, Vector3 facePos)
    {
        vertices.AddRange(FaceVertices((int)dir, scale, facePos));
        int vertexCount = vertices.Count;
        triangles.Add(vertexCount - 4);
        triangles.Add(vertexCount - 4 + 1);
        triangles.Add(vertexCount - 4 + 2);
        triangles.Add(vertexCount - 4);
        triangles.Add(vertexCount - 4 + 2);
        triangles.Add(vertexCount - 4 + 3);
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
