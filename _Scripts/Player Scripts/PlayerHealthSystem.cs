using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour
{
    public PlayerStats playerStats;

    private void Start()
    {
        playerStats.currentHealth = playerStats.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        float defense = playerStats.GetDefense();
        float actualDamage = damage - defense;
        if (actualDamage < 0) actualDamage = 0;

        playerStats.currentHealth -= actualDamage;

        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerStats.ResetStats();
        // TODO: Handle player death
        // Destroy(gameObject);
        // and load camp scene
        SceneManager.LoadScene("Camp");
        // Handle player death
    }

    public void Heal(int amount)
    {
        playerStats.currentHealth += amount;
        if (playerStats.currentHealth > playerStats.maxHealth)
        {
            playerStats.currentHealth = playerStats.maxHealth;
        }
    }
}
