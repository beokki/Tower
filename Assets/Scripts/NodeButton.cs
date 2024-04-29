using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeButton : MonoBehaviour
{
    public enum NodeState
    {
        Acquired,
        Available,
        Unavailable
    }

    [SerializeField] NodeButton parentNode;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TMP_Text text;
    [SerializeField] int requiredWave = 0;
    [SerializeField] Bonus bonus;

    LineRenderer lineRenderer;
    public NodeState currentState = NodeState.Unavailable;

    List<NodeButton> children = new List<NodeButton>();

    private void Awake()
    {
        text.text = bonus.GetDescription();
        if (parentNode != null)
        {
            parentNode.children.Add(this);
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, parentNode.transform.position);
        }
    }

    private void Start()
    {
        if (parentNode == null || parentNode.currentState == NodeState.Acquired)
        {
            SetState(NodeState.Available);
        }
        else
        {
            SetState(NodeState.Unavailable);
        }
    }

    public void SetState(NodeState nodeState)
    {
        if (nodeState == NodeState.Available && !CanUnlock())
            return;

        currentState = nodeState;
        switch (currentState)
        {
            case NodeState.Acquired:
                ApplyBonus();

                spriteRenderer.color = Color.green;
                text.text = bonus.GetDescription();
                foreach (var child in children)
                {
                    if (child.CanUnlock())
                    {
                        child.SetState(NodeState.Available);
                    }
                }
                break;
            case NodeState.Available:
                spriteRenderer.color = Color.white;
                foreach (var child in children)
                {
                    child.SetState(NodeState.Available);
                }
                break;
            case NodeState.Unavailable:
                spriteRenderer.color = Color.red;
                break;
        }
    }

    private void ApplyBonus()
    {
        switch (bonus.type)
        {
            case Bonus.BonusType.HealthBoost:
                GameManager.instance.UpdateHealth((int)bonus.value);
                break;
            case Bonus.BonusType.TurretFireRate:
                GameManager.instance.ApplyBonusToTurrets(tile => tile.IncreaseFR(bonus.value));
                break;
            case Bonus.BonusType.TurretRange:
                GameManager.instance.ApplyBonusToTurrets(tile => tile.IncreaseRange((int)bonus.value));
                break;
            case Bonus.BonusType.TurretDamage:
                GameManager.instance.ApplyBonusToTurrets(tile => tile.IncreaseDamage((int)bonus.value));
                break;
        }
    }


    private void TurretBonus(Action<GameTile> apply)
    {
        GameTile[] tiles = FindObjectsOfType<GameTile>();
        foreach (GameTile tile in tiles)
        {
            if (tile.turretRenderer.enabled)
            {
                apply(tile);
            }
        }
    }

    public void UpdateNodeAvailability(int currentWave)
    {
        if (currentState == NodeState.Unavailable && CanUnlock())
        {
            SetState(NodeState.Available);
        }
    }

    private bool CanUnlock()
    {
        return GameManager.instance.currentWave >= requiredWave;
    }

    private void OnMouseDown()
    {
        if (currentState == NodeState.Available)
        {
            SetState(NodeState.Acquired);
        }
        else if (currentState == NodeState.Unavailable && parentNode != null && parentNode.currentState == NodeState.Acquired)
        {
            SetState(NodeState.Available);
        }
    }

    [System.Serializable]
    public class Bonus
    {
        public enum BonusType
        {
            HealthBoost,
            TurretFireRate,
            TurretRange,
            TurretDamage
        }

        public BonusType type;
        public float value;

        public string GetDescription()
        {
            switch (type)
            {
                case BonusType.HealthBoost:
                    return $"Health: +{value}";
                case BonusType.TurretFireRate:
                    return $"Fire Rate: +{value}";
                case BonusType.TurretRange:
                    return $"Rang:e +{value}";
                case BonusType.TurretDamage:
                    return $"Damage: +{value}";
                default:
                    return "Unknown Bonus";
            }
        }
    }
}
