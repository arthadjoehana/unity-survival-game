using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyStatsReference", menuName = "EnemyStatsReference")]
public class EnemyStatsReference : ScriptableObject
{
    public EnemyStats _enemyStats { get => _enemyStats; set => _enemyStats = value; }

    [Header("Other elements references")]
    [SerializeField] private Animator animator;
    //[SerializeField] private MoveBehaviour playerMovementScript;
    //[SerializeField] private AimBehaviourBasic playerAimScript;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;
    public float healthDecreaseRateForHungerAndThirst;

    public float damage;



    public float stealthDetection;

    [HideInInspector]
    public bool isDead = false;
}
