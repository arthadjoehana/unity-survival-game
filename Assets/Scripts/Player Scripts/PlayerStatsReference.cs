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
    PlayerHealth _playerHealth;

    public PlayerStats PlayerStats { get => _playerStats; set => _playerStats = value; }
    public PlayerMovement PlayerMovement { get => _playerMovement; set => _playerMovement = value; }
    public PlayerHealth PlayerHealth { get => _playerHealth; set => _playerHealth = value; }

    public float maxHealth;
    public float currentHealth;

    public float attack;
    public float defense;
    public float agility;
    public float dexterity;
    public float intelligence;

    public int level;
    public float exp;
    public int talentPoints;

    public bool isDead;
    public bool isBleeding;
    public bool isStunned;

    public float currentArmorPoints;

    public float baseStealth;
    public float crouchStealth;

}