using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAgent : TerrainAgent
{
    public int upperTreeHeigt;
    public int lowerTreeHeight;
    private Transform Trees;
    private Transform Rocks;
    public float treeSpawnChance = 1 / 12f;
    public float rockSpawnChance = 1 / 25f;
    public GameObject[] trees;
    public GameObject[] rocks;

    public override void UpdateGrid(VoxelGrid grid)
    {
        Trees = GameObject.Find("Trees").GetComponent<Transform>();
        Trees.DestroyChildren();
        Rocks = GameObject.Find("Rocks").GetComponent<Transform>();
        Rocks.DestroyChildren();
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = lowerTreeHeight; y < upperTreeHeigt; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    if (grid.GetDepth(position.x, position.y, position.z) == 1 && grid.GetCell(position.x, position.y, position.z) == 2)
                    {
                        if (Random.Range(0f, 1f) < treeSpawnChance)
                        {
                            position.y++;
                            GameObject tree;
                            int randTreeIdx = Random.Range(0, trees.Length);
                            tree = Instantiate(trees[randTreeIdx], position, Quaternion.identity, Trees);
                            tree.transform.Rotate(-90f, Random.Range(0, 360f), 0f, Space.Self);
                        }
                    }
                    if (grid.GetDepth(position.x, position.y, position.z) == 0 && grid.GetCell(position.x, position.y, position.z) == 2 && grid.GetMaxSlope(position, 3) < 7)
                    {
                        if (Random.Range(0f, 1f) < rockSpawnChance)
                        {
                            GameObject Rock;
                            int select = Random.Range(0, rocks.Length);
                            Rock = Instantiate(rocks[select], position, Quaternion.identity, Rocks);
                            Rock.transform.Rotate(Vector3.Scale(Random.onUnitSphere, new Vector3(180, 180, 180)));
                        }
                    }
                }
            }
        }
     
    }
}
