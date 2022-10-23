using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerStatsReference", menuName = "PlayerStatsReference")]
public class PlayerStatsReference : ScriptableObject
{
    public PlayerStats _playerStats { get => _playerStats; set => _playerStats = value; }

    [Header("Other elements references")]
    [SerializeField] private Animator animator;
    //[SerializeField] private MoveBehaviour playerMovementScript;
    //[SerializeField] private AimBehaviourBasic playerAimScript;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;
    public float healthDecreaseRateForHungerAndThirst;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float currentHunger;
    public Image hungerBarFill;
    public float hungerDecreaseRate;

    [Header("Thirst")]
    public float maxThirst = 100f;
    public float currentThirst;
    public Image thirstBarFill;
    public float thirstDecreaseRate;

    [Header("Sleep")]
    public float maxSleep = 100f;
    public float currentSleep;
    public Image SleepBarFill;
    public float SleepDecreaseRate;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public Image StaminaBarFill;
    public float StaminaDecreaseRate;

    public float currentArmorPoints;

    public float stealth;
    public float crouchStealth;

    [HideInInspector]
    public bool isDead = false;
}