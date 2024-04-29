using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 20;
    public int maxhp = 20;

    public static HashSet<Enemy> enemies = new HashSet<Enemy>();

    public GameObject deathEffect;

    //private Renderer rend;
    //public Color fullHP = Color.green;
    //public Color lowHP = Color.red;

    public float speed = 1f;

    private readonly Stack<GameTile> path = new Stack<GameTile>();

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
        //rend = GetComponent<Renderer>();
        //rend.material.color = fullHP;
    }

    void Update()
    {
        if (path.Count > 0)
        {
            Vector3 desPos = path.Peek().transform.position;
            transform.position = Vector3.MoveTowards(transform.position, desPos, speed * Time.deltaTime);

            Vector3 direction = desPos - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            }

            if (Vector3.Distance(transform.position, desPos) < 0.01f)
            {
                path.Pop();
            }
        }
        else
        {
            Die();
            Player.instance.TakeDamage(1);
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        //UpdateColor();
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        enemies.Remove(this);
        Destroy(gameObject);
        GameManager.instance.EnemyDefeated();
    }

    //private void UpdateColor()
    //{
    //    float hpratio = (float)hp / maxhp;
    //    rend.material.color = Color.Lerp(lowHP, fullHP, hpratio);
    //}
}
