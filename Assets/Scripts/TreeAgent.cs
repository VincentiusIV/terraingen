using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAgent : TerrainAgent
{
    public int upperTreeHeigt;
    public int lowerTreeHeight;
    public GameObject treeA;
    public GameObject treeB;
    public GameObject Rock1;
    public GameObject Rock2;
    public GameObject Rock3;
    public GameObject Rock4;
    public GameObject Rock5;
    public GameObject Rock6;

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
                    if (grid.GetDepth(position.x, position.y, position.z) == 0 && grid.GetCell(position.x, position.y, position.z) == 2 && grid.GetMaxSlope(position, 3) < 7)
                    {
                        if (Random.Range(0, 25) < 1)
                        {
                            GameObject Rock;
                            int select = (int)Random.Range(0, 6);
                            switch (select)
                            {
                                case 0:
                                    Rock = Instantiate(Rock1, position, Quaternion.identity);
                                    break;
                                case 1:
                                    Rock = Instantiate(Rock2, position, Quaternion.identity);
                                    break;
                                case 2:
                                    Rock = Instantiate(Rock3, position, Quaternion.identity);
                                    break;
                                case 3:
                                    Rock = Instantiate(Rock4, position, Quaternion.identity);
                                    break;
                                case 4:
                                    Rock = Instantiate(Rock5, position, Quaternion.identity);
                                    break;
                                case 5:
                                    Rock = Instantiate(Rock6, position, Quaternion.identity);
                                    break;
                                default:
                                    Rock = Instantiate(Rock1, position, Quaternion.identity);
                                    break;
                            }
                            Rock.transform.Rotate(Vector3.Scale(Random.onUnitSphere, new Vector3(180, 180, 180)));
                        }
                    }
                }
            }
        }
     
    }
}
