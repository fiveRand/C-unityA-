using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar 
{
    public static List<AstarPoint> FindPath(AStarGrid grid, Vector2Int startIndex, Vector2Int endIndex)
    {
        List<AstarNode> temp = tempPath(grid, startIndex, endIndex);

        List<AstarPoint> result = new List<AstarPoint>(temp.Count);
        for(int i =0; i < temp.Count; i++)
        {
            result.Add(new AstarPoint(temp[i]));
        }
        return result;
    }

    public static Queue<AstarPoint> FindPathQueue(AStarGrid grid, Vector2Int startIndex, Vector2Int endIndex)
    {
        List<AstarNode> temp = tempPath(grid, startIndex, endIndex);
        Queue<AstarPoint> pp = new Queue<AstarPoint>(temp.Count);

        for (int i = 0; i < temp.Count; i++)
        {
            pp.Enqueue(new AstarPoint(temp[i]));
        }

        return pp;
    }


    static List<AstarNode> tempPath(AStarGrid grid,Vector2Int startIndex,Vector2Int endIndex)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        AstarNode startNode = grid.GetNode(startIndex);
        AstarNode endNode = grid.GetNode(endIndex);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or target node is null");
            return null;
        }

        BinaryHeap<AstarNode> openSet = new BinaryHeap<AstarNode>(grid.MaxSize());
        HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
        // HashSet 와 List 비교
        // 중복멤버 차단
        // 검색 최적화
        // 인덱스 사용 불가
        List<AstarNode> result = null;

        openSet.Enqueue(startNode);

        while (openSet.Length > 0)
        {
            AstarNode curNode = openSet.Dequeue();
            closedSet.Add(curNode);

            if (curNode == endNode)
            {
                
                result = RetracePath(startNode, endNode);
                Debug.Log("Time : " + sw.ElapsedMilliseconds);
                sw.Reset();
                break;
            }

            foreach (AstarNode neighbor in grid.GetNeighbours(curNode))
            {
                if (closedSet.Contains(neighbor)) continue;

                int newCost2Neighbor = curNode.Gcost + GetDistance(curNode, neighbor) + neighbor.penalty;

                if (newCost2Neighbor < neighbor.Gcost || openSet.Contains(neighbor) == false)
                {
                    neighbor.Gcost = newCost2Neighbor;
                    neighbor.Hcost = GetDistance(neighbor, endNode);
                    neighbor.parent = curNode;

                    if (openSet.Contains(neighbor) == false)
                    {
                        openSet.Enqueue(neighbor);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                    }
                }
            }
        }
        return result;
    }

    static int GetDistance(AstarNode nodeA, AstarNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        else
            return 14 * dstX + 10 * (dstY - dstX);
    }

    static List<AstarNode> RetracePath(AstarNode startNode, AstarNode endNode)
    {
        List<AstarNode> path = new List<AstarNode>();
        AstarNode curNode = endNode;

        while (curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }

        path.Reverse();
        return path;
    }
}

public struct AstarPoint
{
    public int x;
    public int y;
    public Vector3 pos;

    public AstarPoint(int x_,int y_,Vector3 pos_)
    {
        x = x_;
        y = y_;
        pos = pos_;
    }

    public AstarPoint(AstarNode node)
    {
        x = node.x;
        y = node.y;
        pos = node.position;
    }
}