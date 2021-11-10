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
    public float maxTreeSlope = 2f;
    public float minDistFromVolcano = 5;
    

    public GameObject[] trees;
    public GameObject[] rocks;

    private List<(Vector3, float)> volcanoPositions;

    public override void UpdateGrid(VoxelGrid grid)
    {
        if(Loader.treeSpawn != 0)
        {
            treeSpawnChance = Loader.treeSpawn / 100;
        }
        if (Loader.rockSpawn != 0)
        {
            rockSpawnChance = Loader.rockSpawn / 100;
        }
        VolcanoAgent volcanoAgent = GameObject.Find("VolcanoAgent").GetComponent<VolcanoAgent>();
        volcanoPositions = volcanoAgent.GetPositions();
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

                    
                    if (grid.GetDepth(position.x, position.y, position.z) == 1 && grid.GetCell(position.x, position.y, position.z) == 2 && DistToVolcano(position) > minDistFromVolcano)
                    {
                        float slope = grid.GetMaxSlope(position);
                        if (slope > maxTreeSlope)
                            continue;
                        if (Random.Range(0f, 1f) < treeSpawnChance)
                        {
                            position.y++;
                            GameObject tree;
                            int randTreeIdx = Random.Range(0, trees.Length);
                            tree = Instantiate(trees[randTreeIdx], position, Quaternion.identity, Trees);
                            tree.transform.Rotate(-90f, Random.Range(0, 360f), 0f, Space.Self);
                        }
                    }
                    if (grid.GetDepth(position.x, position.y, position.z) == 1 && grid.GetCell(position.x, position.y, position.z) == 2 && grid.GetMaxSlope(position, 3) < 7)
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

    private float DistToVolcano(Vector3Int pos)
    {
        if (volcanoPositions.Count != 0)
        {
            float distance = float.MaxValue;
            foreach (var volcano in volcanoPositions)
            {
                Vector3 volcPos = volcano.Item1;
                float radius = volcano.Item2;

                float xSq = Mathf.Pow(Mathf.Abs(pos.x - volcPos.x), 2);
                float ySq = Mathf.Pow(Mathf.Abs(pos.y - volcPos.y), 2);
                float zSq = Mathf.Pow(Mathf.Abs(pos.z - volcPos.z), 2);

                float currentDist = Mathf.Sqrt(xSq + ySq + zSq);
                if (currentDist < distance)
                {
                    distance = currentDist - radius;
                }
            }
            return distance;
        }
        else
        {
            return float.MaxValue;
        }

    }

}
