using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public float nodeDiameter;
    public TerrainType[] terrain;
    public Vector2 gridSize;
    Vector2Int gridIndex;
    AstarNode[,] nodes;
    Vector3 gridPos;

    [SerializeField] bool showGizmos;
    [SerializeField] bool showWeights;

    bool isInitialized;

    private void Awake()
    {
        int gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        int gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        gridPos = this.transform.position;
        gridIndex = new Vector2Int(gridSizeX, gridSizeY);
        nodes = new AstarNode[gridSizeX, gridSizeY];

        CreateGrid();

        isInitialized = true;
    }

    public int MaxSize()
    {
        return gridIndex.x * gridIndex.y;
    }
    void CreateGrid()
    {
       
        Vector3 wbl = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;

        for(int x = 0; x < gridIndex.x; x++)
        {
            for(int y = 0; y < gridIndex.y; y++)
            {
                Vector3 offsetX = Vector3.right * (x * nodeDiameter + nodeDiameter * 0.5f);
                Vector3 offsetY = Vector3.up * (y * nodeDiameter + nodeDiameter * 0.5f);
                Vector3 pos = wbl + offsetX + offsetY;

                int penalty = 0;
                bool imPassable = false;
                foreach(TerrainType region in terrain)
                {
                    bool hit = Physics2D.OverlapBox(pos, Vector2.one * nodeDiameter * 0.9f, 0, region.terrainLayer);

                    if (hit)
                    {
                        penalty = region.terrainPenalty;
                        imPassable = region.impassable;
                    }
                }

                nodes[x, y] = new AstarNode(pos, x, y, penalty,imPassable);
            }
        }

    }

    public void UpdateGrid()
    {
        Vector3 wbl = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;

        for (int x = 0; x < gridIndex.x; x++)
        {
            for (int y = 0; y < gridIndex.y; y++)
            {
                Vector3 offsetX = Vector3.right * (x * nodeDiameter + nodeDiameter * 0.5f);
                Vector3 offsetY = Vector3.up * (y * nodeDiameter + nodeDiameter * 0.5f);
                Vector3 pos = wbl + offsetX + offsetY;

                int penalty = 0;
                bool imPassable = false;
                foreach (TerrainType region in terrain)
                {
                    bool hit = Physics2D.OverlapBox(pos, Vector2.one * nodeDiameter, 0, region.terrainLayer);

                    if (hit)
                    {
                        penalty = region.terrainPenalty;
                        imPassable = region.impassable;
                    }
                }

                nodes[x, y].imPassable = imPassable;
                nodes[x, y].penalty = penalty;
            }
        }
    }

    int penaltyMax = int.MinValue;
    int penaltyMin = int.MaxValue;
    /*
    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHor = new int[gridIndex.x, gridIndex.y];
        int[,] penaltiesVer = new int[gridIndex.x, gridIndex.y];

        for(int y = 0; y < gridIndex.y; y++)
        {
            for(int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHor[0, y] += nodes[sampleX, y].penalty;
            }

            for(int x= 1; x < gridIndex.x; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridIndex.x);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridIndex.x-1);

                penaltiesHor[x, y] = penaltiesHor[x - 1, y] - nodes[removeIndex, y].penalty + nodes[addIndex, y].penalty;
            }
        }

        for(int x = 0; x < gridIndex.x;x++)
        {
            for(int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVer[x, 0] += nodes[x, sampleY].penalty;
            }


            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVer[x, 0] / (kernelSize * kernelSize));
            nodes[x, 0].penalty = blurredPenalty;

            for(int y= 1; y < gridIndex.y; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridIndex.y);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridIndex.y - 1);

                penaltiesVer[x, y] = penaltiesVer[x, y - 1] - penaltiesHor[x, removeIndex] + penaltiesHor[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVer[x, y] / (kernelSize * kernelSize));
                nodes[x, y].penalty = blurredPenalty;


                if(blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }
    }
    */
    public List<AstarNode> GetNeighbours(AstarNode node)
    {
        List<AstarNode> neighbours = new List<AstarNode>(4);

        if (node == null) return null;

        Vector2Int left = new Vector2Int(node.x - 1, node.y);
        Vector2Int right = new Vector2Int(node.x + 1, node.y);
        Vector2Int up = new Vector2Int(node.x, node.y + 1);
        Vector2Int down = new Vector2Int(node.x , node.y - 1);

        if(isValid(left))
        {
            neighbours.Add(GetNode(left));
        }
        if(isValid(right))
        {
            neighbours.Add(GetNode(right));
        }
        if(isValid(up))
        {
            neighbours.Add(GetNode(up));
        }
        if(isValid(down))
        {
            neighbours.Add(GetNode(down));
        }

        return neighbours;
    }

    public AstarNode GetNodeFromWorldPosition(Vector3 position)
    {

        return GetNode(GetIndexFromWorldPos(position));
    }

    public Vector2Int GetIndexFromWorldPos(Vector3 position)
    {
        float percentX = (position.x + gridSize.x * 0.5f - gridPos.x) / gridSize.x;
        float percentY = (position.y + gridSize.y * 0.5f - gridPos.y) / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridIndex.x) * percentX);
        int y = Mathf.RoundToInt((gridIndex.y) * percentY);

        return new Vector2Int(x, y);
    }

    public bool isInBoundary(int x, int y)
    {
        if((x >= 0) && (x < gridIndex.x) && (y >= 0) && (y < gridIndex.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isValid(int x,int y)
    {
        if(isInBoundary(x,y))
        {
            if(nodes[x,y].imPassable == false)
            {
                return true;
            }
        }
        return false;
    }

    public bool isValid(Vector2Int index)
    {
        return isValid(index.x, index.y);
    }

    public AstarNode GetNode(Vector2Int index)
    {
        if(isValid(index))
        {
            return nodes[index.x, index.y];
        }
        else // search ± 1 around to find isnt a wall
        {
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int aroundx = index.x + x;
                    int aroundy = index.y + y;

                    if (isValid(aroundx, aroundy))
                    {
                        return nodes[aroundx, aroundy];
                    }

                    if (x == 0 && y == 0) continue;

                }
            }
        }



        return null;
    }

    private void OnDrawGizmos()
    {

        if (showGizmos)
        {
            Gizmos.DrawWireCube(transform.position, gridSize);

            if (isInitialized)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;

                Color color = Color.white;
                foreach (AstarNode node in nodes)
                {

                    if (showWeights)
                    {
                        UnityEditor.Handles.Label(node.position, node.penalty.ToString(), style);
                    }

                    if(node.imPassable)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(0, byte.MaxValue, node.penalty)); ;
                    }
                    Gizmos.DrawWireCube(node.position, Vector3.one * nodeDiameter);
                }
            }
        }
    }
}



public class AstarNode
{
    public AstarNode parent;
    public Vector3 position;
    public int penalty;
    public bool imPassable;
    public int x;
    public int y;

    public int Gcost;
    public int Hcost;
    public int Fcost { get { return Gcost + Hcost; } }


    int heapIndex;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public AstarNode(Vector3 _position, int _x, int _y, int _penalty,bool _imPassable)
    {
        position = _position;
        x = _x;
        y = _y;
        penalty = _penalty;
        imPassable = _imPassable;
        heapIndex = 0;
    }

}

[System.Serializable]
public struct TerrainType
{
    public LayerMask terrainLayer;
    public bool impassable;
    [Range(0, 255)]
    public int terrainPenalty;
}
