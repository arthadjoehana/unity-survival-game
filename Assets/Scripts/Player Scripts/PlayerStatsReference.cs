using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerStatsReference", menuName = "PlayerStatsReference")]
public class PlayerStatsReference : ScriptableObject
{
    PlayerStats _playerStats;
    PlayerMovement _playerMovement;

    public PlayerStats PlayerStats { get => _playerStats; set => _playerStats = value; }
    public PlayerMovement PlayerMovement { get => _playerMovement; set => _playerMovement = value; }

    public float maxHealth;
    public float currentHealth;

    public float currentArmorPoints;

    public float baseStealth;
    public float crouchStealth;

    public float attack;

    public bool isDead = false;
}