using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;

    private int health = 10;
    public static int bonusHP = 0;
    public static Player instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        health = health + bonusHP;
        hpText.text = $"HP: {health}";
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        hpText.text = $"HP: {health}";
    }
}
