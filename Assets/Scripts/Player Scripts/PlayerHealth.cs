using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] HealthBar _healthBar;

    public float playerHealth;
    public float CurrentHealth { get => playerHealth; }
    public float MaxHealth { get => _playerStatsRef.maxHealth; }

    public void Awake()
    {
        _playerStatsRef.PlayerHealth = this;
        _playerStatsRef.currentHealth = _playerStatsRef.maxHealth;
    }
    public void Start()
    {
        playerHealth = _playerStatsRef.currentHealth;
        _healthBar.UpdateHealthBar(this);
    }

    public void TakeDamage(float damage)
    {
        _playerStatsRef.currentHealth -= damage;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        playerHealth = _playerStatsRef.currentHealth;
        _healthBar.UpdateHealthBar(this);
    }
}
