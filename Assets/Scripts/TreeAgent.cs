using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAgent : TerrainAgent
{
    public int upperTreeHeigt;
    public int lowerTreeHeight;
    public GameObject treeA;
    public GameObject treeB;

    public override void UpdateGrid(VoxelGrid grid)
    {
        treeA.transform.Rotate(-90f, 0f, 0f, Space.Self);
        for (int x = 0; x < grid.Width; x++)
        {
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = lowerTreeHeight; y < upperTreeHeigt; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    if (grid.GetDepth(position.x, position.y, position.z) == 0 && grid.GetCell(position.x, position.y, position.z) == 2 && grid.GetMaxSlope(position, 3) < 7)
                    {
                        if(Random.Range(0,8) < 1)
                        {
                            position.y++;
                            GameObject tree;
                            if(Random.Range(0, 2) < 1)
                            {
                                tree = Instantiate(treeA, position, Quaternion.identity);
                            }
                            else
                            {
                                tree = Instantiate(treeB, position, Quaternion.identity);
                            }
                            tree.transform.Rotate(-90f, 0f, 0f, Space.Self);
                        }
                    }
                }
            }
        }
     
    }
}
