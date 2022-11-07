using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] EnemyHealthBar _enemyHealthBar;

    public float maxHealth;
    public float currentHealth;

    public float enemyHealth;
    public float CurrentHealth { get => enemyHealth; }
    public float MaxHealth { get => _enemyStatsRef.maxHealth; }

    public void Awake()
    {
        maxHealth = _enemyStatsRef.maxHealth;
        currentHealth = _enemyStatsRef.maxHealth;
    }

    private void Start()
    {
        enemyHealth = currentHealth;
        _enemyHealthBar.UpdateHealthBar(this);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        enemyHealth = currentHealth;
        _enemyHealthBar.UpdateHealthBar(this);
    }
}
