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
    [SerializeField] HealthUI _healthUI;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] EnemyAI _enemyAI;

    private bool isAlive;

    public float playerHealth;
    public float Health { get => playerHealth; }

    public void Awake()
    {
        isAlive = true;
        _playerStatsRef.PlayerHealth = this;
        _playerStatsRef.currentHealth = _playerStatsRef.maxHealth;
    }
    public void Start()
    {
        playerHealth = _playerStatsRef.currentHealth;
        _healthUI.UpdateHealthText(this);
    }

    public void Update()
    {
        /*if (playerHealth <= 0 && isAlive)
        {
            _playerStatsRef.currentHealth = 0;
            _playerMovement.Die();
            isAlive = false;
        }*/
    }

    public void TakeDamage()
    {
        
        float damage = _enemyAI.damage;
        _playerStatsRef.currentHealth -= damage;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        playerHealth = _playerStatsRef.currentHealth;
        _healthUI.UpdateHealthText(this);
    }
}
