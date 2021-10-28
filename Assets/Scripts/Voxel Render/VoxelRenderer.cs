using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRenderer : MonoBehaviour
{
    public float scale = 1f;

    private MeshFilter meshFilter;
    private Mesh mesh => Application.isPlaying ? meshFilter.mesh : meshFilter.sharedMesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private float adjScale;

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

    private void Awake()
    {
        
    }

    private void Start()
    {
        GenerateDefault();
    }

    [ContextMenu("GenerateDefault")]
    private void GenerateDefault()
    {
        meshFilter = GetComponent<MeshFilter>();
        adjScale = scale * 0.5f;
        CreateVoxelMesh(new VoxelGrid());
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
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
