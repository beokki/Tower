using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTile : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerDownHandler
{
    [SerializeField] SpriteRenderer hoverRenderer;
    [SerializeField] public SpriteRenderer turretRenderer;
    [SerializeField] SpriteRenderer spawnRenderer;

    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;

    private Color originalColor;

    [SerializeField] public int damage = 10;
    [SerializeField] public float fireRate = 1f;
    [SerializeField] public float fireCooldown = 0f;
    [SerializeField] public int range = 2;

    public GameManager GM { get; internal set; }
    public int X { get; internal set; }
    public int Y { get; internal set; }
    public bool IsBlocked { get; private set; }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.SetPosition(0, transform.position);

        spriteRenderer = GetComponent<SpriteRenderer>();
        turretRenderer.enabled = false;
        originalColor = spriteRenderer.color;

    }

    private void Update()
    {
        if (turretRenderer.enabled)
        {
            Rotate();

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
            else
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    private void Rotate()
    {
        Enemy target = CloseEnemy();
        if (target != null)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            turretRenderer.transform.rotation = Quaternion.Euler(0, 0, angle - 90);


            lineRenderer.SetPosition(1, target.transform.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private Enemy CloseEnemy()
    {
        Enemy target = null;
        float closeDistance = Mathf.Infinity;

        foreach (var enemy in Enemy.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < closeDistance)
            {
                target = enemy;
                closeDistance = distance;
            }
        }
        return target;
    }

    private void Shoot()
    {
        Enemy target = CloseEnemy();

        if (target != null)
        {
            lineRenderer.enabled = true;

            target.TakeDamage(damage);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    internal void TurnGray()
    {
        spriteRenderer.color = new Color32(255, 255, 255, 5);
        originalColor = spriteRenderer.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverRenderer.enabled = true;
        GM.TargetTile = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverRenderer.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        turretRenderer.enabled = !turretRenderer.enabled;
        IsBlocked = turretRenderer.enabled;
        if (turretRenderer.enabled)
        {
            GameManager.instance.RegisterTurret(this);
        }
        else
        {
            GameManager.instance.UnregisterTurret(this);
        }
    }

    internal void SetEnemySpawn()
    {
        spawnRenderer.enabled = true;
    }

    internal void SetPath(bool isPath)
    {
        spriteRenderer.color = isPath ? new Color(255, 255, 255, 5) : originalColor;
    }

    public void IncreaseDamage(int inc)
    {
        damage += inc;
    }

    public void IncreaseFR(float inc)
    {
        fireRate += inc;
    }

    public void IncreaseRange(int inc)
    {
        range += inc;
    }
}
