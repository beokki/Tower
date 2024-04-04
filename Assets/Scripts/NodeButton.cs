using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeButton : MonoBehaviour
{
    public enum NodeState
    {
        Aquired,
        Available,
        Unavailable
    }

    [SerializeField] NodeButton parentNode;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TMP_Text text;
    [SerializeField] int bonusHP = 1;
    LineRenderer lineRenderer;
    NodeState currentState = NodeState.Unavailable;

    List<NodeButton> children = new List<NodeButton>();

    private void Awake()
    {
        text.text = $"{bonusHP} HP";
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
        if (parentNode == null)
        {
            SetState(NodeState.Available);
        }
    }

    private void SetState(NodeState nodeState)
    {
        currentState = nodeState;
        switch (currentState)
        {
            case NodeState.Aquired:
                Player.bonusHP += bonusHP;

                spriteRenderer.color = Color.green;
                foreach (var child in children)
                {
                    child.SetState(NodeState.Available);
                }
                break;
            case NodeState.Available:
                spriteRenderer.color = Color.white;
                foreach (var child in children)
                {
                    child.SetState(NodeState.Unavailable);
                }
                break;
            case NodeState.Unavailable:
                spriteRenderer.color = Color.red;
                foreach (var child in children)
                {
                    child.SetState(NodeState.Unavailable);
                }
                break;
        }
    }

    private void OnMouseDown()
    {
        if (currentState == NodeState.Available)
        {
            SetState(NodeState.Aquired);
        }
    }
}
