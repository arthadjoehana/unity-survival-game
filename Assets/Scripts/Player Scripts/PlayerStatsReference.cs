using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerStatsReference", menuName = "PlayerStatsReference")]
public class PlayerStatsReference : ScriptableObject
{
    PlayerMovement _playerMovement;
    PlayerHealth _playerHealth;
    PlayerLevel _playerLevel;

    public PlayerMovement PlayerMovement { get => _playerMovement; set => _playerMovement = value; }
    public PlayerHealth PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
    public PlayerLevel PlayerLevel { get => _playerLevel; set => _playerLevel = value; }

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