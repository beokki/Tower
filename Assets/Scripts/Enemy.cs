using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static HashSet<Enemy> enemies = new HashSet<Enemy>();

    private Stack<GameTile> path = new Stack<GameTile>();

    internal void SetPath(List<GameTile> pathEnd)
    {
        path.Clear();
        foreach (GameTile tile in pathEnd)
        {
            path.Push(tile);
        }
    }

    private void Awake()
    {
        enemies.Add(this);
    }

    void Update()
    {
        if (path.Count > 0)
        {
            Vector3 desPos = path.Peek().transform.position;
            transform.position = Vector3.MoveTowards(transform.position, desPos, 2 * Time.deltaTime);

            if (Vector3.Distance(transform.position, desPos) < 0.01f)
            {
                path.Pop();
            }
        }
        else
        {
            enemies.Remove(this);
            Destroy(gameObject);
        }
    }
}
