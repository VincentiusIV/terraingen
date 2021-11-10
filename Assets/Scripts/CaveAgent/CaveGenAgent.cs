using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaveGenAgent : TerrainAgent
{

    public int tokens = 100;
    public int maxCaves = Loader.caves;
    private int generatedCaves = 0;
    public override void UpdateGrid(VoxelGrid grid)
    {
        if(Application.isPlaying)
            maxCaves = Loader.caves;
        for (int i = 0; i < tokens; i++)
        {
            if (generatedCaves < maxCaves)
            {
                int x = Random.Range(5, grid.Width - 5);
                int y = Random.Range(5, (int)grid.Height / 4 - 5);
                int z = Random.Range(5, grid.Depth - 5);
                Vector3Int randomPos = new Vector3Int(x, y, z);
                if (PositionValid(randomPos, grid))
                {
                    string axiom = GenerateAxiom(20, true);
                    string rule = GenerateAxiom(4, false);
                    Lsystem lsystem = new Lsystem(axiom, rule, grid, randomPos);
                    lsystem.Generate();
                    grid = lsystem.grid;
                    //Debug.Log("Generated Cave");
                    generatedCaves++;
                }
            }
        }
    }

    private bool PositionValid(Vector3Int position, VoxelGrid grid) //A valid starting positions is in the center of an encased 5x5x5 cube.
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        for (int x = -2; x < 3; x++)
        {
            for (int y = -2; y < 3; y++)
            {
                for (int z = -2; z < 3; z++)
                {
                    if (grid.GetCell(position.x - x, position.y - y, position.z - z) == 0)
                    {
                        cells.Add(new Vector3Int(position.x - x, position.y - y, position.z - z));
                    }
                }
            }
        }
        if (cells.Count == 0)
        {
            return true;
        }
        return false;
    }

    private string GenerateAxiom(int length, bool allowSplit)
    {
        int axiomLength = Random.Range(length-5, length);
        string axiom = "";
        for (int i = 0; i < axiomLength; i++)
        {
            if (Random.Range(0, 3) > 1 && allowSplit)
            {
                axiom += "[";
                for (int j = 0; j < (int)Random.Range(1, 4); j++)
                {
                    axiom += addChar();
                }
                axiom += "]";
            }
            axiom += addChar();
        }
        return axiom;
    }

    private string addChar()
    {
        string axiom = "";
        int nextChar = Random.Range(0, 8);
        switch (nextChar)
        {
            case 0:
                axiom += "X";
                break;
            case 1:
                axiom += "x";
                break;
            case 2:
                axiom += "Y";
                break;
            case 3:
                axiom += "y";
                break;
            case 4:
                axiom += "Z";
                break;
            case 5:
                axiom += "z";
                break;
            default:
                break;
        }
        return axiom;
    }
}

public class Lsystem
{
    private string axiom;
    private string rule;
    public VoxelGrid grid;
    Vector3Int position;

    public Lsystem(string _axiom, string _rule, VoxelGrid _grid, Vector3Int _position)
    {
        axiom = _axiom;
        rule = _rule;
        grid = _grid;
        position = _position;
    }

    public void Generate()
    {
        string _axiom = IterateAxiom(axiom);
        List<Vector3Int> nodes = new List<Vector3Int>();
        for (int i = 0; i < _axiom.Length; i++)
        {
            nodes.Add(position);
            int prefRange = Random.Range(-4, 4);
            int randomRange = Random.Range(-1, 1);
            switch (_axiom[i])
            {
                case 'X':
                    //Move in x+
                    position.x += prefRange;
                    position.y -= randomRange;
                    position.z += randomRange;
                    break;
                case 'x':
                    //Move in x-
                    position.x -= prefRange;
                    position.y += randomRange;
                    position.z -= randomRange;
                    break;
                case 'Y':
                    //Move in y+
                    position.y += prefRange;
                    position.x += randomRange;
                    position.z -= randomRange;
                    break;
                case 'y':
                    //Move in y-
                    position.y -= prefRange;
                    position.x -= randomRange;
                    position.z -= randomRange;
                    break;
                case 'Z':
                    position.z += prefRange;
                    position.y += randomRange;
                    position.x += randomRange;

                    break;
                case 'z':
                    position.z -= prefRange;
                    position.y += randomRange;
                    position.x -= randomRange;
                    break;
                case '[':

                    break;
                case ']':

                    break;

            }
        }
        GenerateCave(nodes);
    }


    private void GenerateCave(List<Vector3Int> nodes)
    {
        HashSet<Vector3Int> cavecells = new HashSet<Vector3Int>();
        foreach (var item in nodes)
        {
            cavecells.UnionWith(generateRandomSphere(item));
        }
        //Debug.Log(cavecells.Count);
        foreach (var cell in cavecells)
        {
            grid.SetCell(cell.x, cell.y, cell.z, 0);
        }
       // Debug.LogFormat("Generated Cave near {0} - {1} - {2}", nodes[0].x, nodes[0].y, nodes[0].z);
    }

    private HashSet<Vector3Int> generateRandomSphere(Vector3Int position)
    {
        HashSet<Vector3Int> caveBlobs = new HashSet<Vector3Int>();
        int genDepth = 1000;
        for (int i = 0; i < genDepth * 3; i++)
        {
            Vector3 randVector = (Random.insideUnitSphere * (int)Random.Range(0, 5));
            randVector.x = randVector.x * Random.Range(0.8f, 1.2f);
            randVector.y = randVector.y * Random.Range(0.8f, 1.2f);
            randVector.z = randVector.z * Random.Range(0.8f, 1.2f);
            if (i > genDepth * 2)
            {
                randVector = (Random.insideUnitSphere * 3);
            }
            else
            {
                randVector = (Random.insideUnitSphere * (int)Random.Range(0, 5));
            }
            Vector3Int chosenPos = new Vector3Int(position.x + (int)randVector.x, position.y + (int)randVector.y, position.z + (int)randVector.z);
            caveBlobs.Add(chosenPos);
        }
        return caveBlobs;

    }

    public string IterateAxiom(string _axiom)
    {
        if (rule != string.Empty)
        {
            string currentAxiom = _axiom;
            char ruleReplace = rule[0];
            int i = 0;
            while (currentAxiom.IndexOf(ruleReplace) != -1)
            {
                string tempCopy = currentAxiom.Replace(ruleReplace, '$');
                currentAxiom = tempCopy;
                i++;
                if (i > 100)
                {
                    Debug.Log("HARDBREAK");
                    break;
                }

            }
            //Debug.Log(currentAxiom);
            string returnAxiom = currentAxiom.Replace("$", rule.Substring(1));
            return returnAxiom;
        }
        else
        {
            return _axiom;
        }
    }

}
