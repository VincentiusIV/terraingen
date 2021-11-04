using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoAgent : TerrainAgent
{
    public int minVolcano = 1, maxVolcano = 1;
    public float minRadius = 10f, maxRadius = 100f;
    public float rimWidth = 0.7f;
    public float rimSteepness = 0.42f;
    public int minFloor = 1, maxFloor = 10;

    public override void UpdateGrid(VoxelGrid grid)
    {
        int volcanoCount = Random.Range(minVolcano, maxVolcano);
        Debug.LogFormat("Generating {0} volcanoes...", volcanoCount);
        for (int i = 0; i < volcanoCount; i++)
        {
            Vector3 volcanoPos = new Vector3(100, 0, 100);// new Vector3(Random.Range(0f, grid.Width - 1), 0f, Random.Range(0f, grid.Depth - 1));
            float volcanoFloorHeight = Random.Range(minFloor, maxFloor);
            float radius = Random.Range(minRadius, maxRadius);
            List<TerrainLayer>[,] layerRepresentation = ErosionAgent.CreateLayerRepresentations(grid);
            for (int x = 0; x < grid.Width; x++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    List<TerrainLayer> layers = layerRepresentation[x, z];
                    Vector3 cell = new Vector3(x , 0f, z);
                    float distanceFromCenter = (cell - volcanoPos).magnitude;
                    float volcanoX = distanceFromCenter / radius;
                    float volcanoShape = Volcano(volcanoX, rimWidth, rimSteepness, -100);
                    for (int j = 0; j < layers.Count; j++)
                    {
                        TerrainLayer layer = layers[j];
                        float prevAmount = layer.amount;
                        layer.amount = prevAmount + (volcanoShape * radius / layers.Count);
                        layers[j] = layer;
                    }
                    layerRepresentation[x, z] = layers;
                }
            }
            ErosionAgent.LayersToVoxels(layerRepresentation, grid);
        }
    }

    float Cavity(float x)
    {
        return x * x - 1;
    }

    float Rim(float x, float rimWidth, float rimSteepness)
    {
        x = Mathf.Min(x - 1f - rimWidth, 0);
        return rimSteepness * x * x;
    }

    float Volcano(float x, float rimWidth, float rimSteepness, float floorHeight)
    {
        return Mathf.Max(Mathf.Min(Cavity(x), Rim(x, rimWidth, rimSteepness)), floorHeight);
    }
}
