using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;

    public int health = 10;
    public static int bonusHP;
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

        health += bonusHP;
        hpText.text = $"HP: {health}";
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        hpText.text = $"HP: {health}";

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ResetHealth()
    {
        health = 10;
        health += bonusHP;
        hpText.text = $"HP: {health}";
    }
}
