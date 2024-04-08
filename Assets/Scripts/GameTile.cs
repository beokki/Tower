using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class GameTile : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerDownHandler
{
    [SerializeField] SpriteRenderer hoverRenderer;
    [SerializeField] SpriteRenderer turretRenderer;
    [SerializeField] SpriteRenderer spawnRenderer;

    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;

    private Color originalColor;

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
            Enemy target = null;
            float closeDistance = Mathf.Infinity;

            foreach (var enemy in Enemy.enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < 2 && distance < closeDistance)
                {
                    target = enemy;
                    closeDistance = distance;
                }
            }

            if (target != null)
            {
                Vector3 targetDir = target.transform.position - transform.position;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                turretRenderer.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                lineRenderer.SetPosition(1, target.transform.position);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    internal void TurnGray()
    {
        spriteRenderer.color = Color.gray;
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
    }

    internal void SetEnemySpawn()
    {
        spawnRenderer.enabled = true;
    }

    internal void SetPath(bool isPath)
    {
        spriteRenderer.color = isPath ? Color.white : originalColor;
    }
}
