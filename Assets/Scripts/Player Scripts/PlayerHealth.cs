using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] HealthUI _healthUI;

    float _playerHealth;
    public float Health { get => _playerHealth; }

    public void Start()
    {
        _playerStatsRef.PlayerHealth = this;
        _playerHealth = _playerStatsRef.maxHealth;
        _healthUI.UpdateHealthText(this);
    }
  
    public void TakeDamage()
    {
        _healthUI.UpdateHealthText(this);
    }
}
