using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarAgent : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] AStarGrid grid;
    Rigidbody2D rb;

    
    int pathIndex;
    Coroutine routine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pathIndex = 0;
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = transform.position;
            Vector2Int start = grid.GetIndexFromWorldPos(pos);
            Vector2Int end = grid.GetIndexFromWorldPos(mousePos);
            var points = Astar.FindPathQueue(grid, start, end);

            pp(points);
        }
    }
    void pp(List<AstarPoint> points)
    {
        if (routine != null) StopCoroutine(routine);
        if (points != null)
        {
            pathIndex = 1;

            routine = StartCoroutine(goPath(points));
        }
    }

    void pp(Queue<AstarPoint> points)
    {
        if (routine != null) StopCoroutine(routine);
        if (points != null)
        {
            routine = StartCoroutine(goPath(points));
        }
    }
    IEnumerator goPath(List<AstarPoint> points)
    {
        while (true)
        {

            Vector2 dir = (points[pathIndex].pos - transform.position).normalized;
            float dist = (points[pathIndex].pos - transform.position).sqrMagnitude;

            if (dist <= 0.5f)
            {
                pathIndex++;
            }

            if (pathIndex == points.Count)
            {
                rb.velocity = Vector2.zero;
                break;
            }
            rb.velocity = dir * moveSpeed;

            yield return null;
        }
    }

    IEnumerator goPath(Queue<AstarPoint> points)
    {
        AstarPoint point = points.Dequeue();
        while (true)
        {
            Vector2 dir = (point.pos - transform.position).normalized;
            float dist = (point.pos - transform.position).sqrMagnitude;

            if (dist <= 0.5f)
            {
                point = points.Dequeue();
            }

            if (points.Count == 0)
            {
                rb.velocity = Vector2.zero;
                break;
            }
            rb.velocity = dir * moveSpeed;

            yield return null;
        }
    }
}
