using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyStatsReference", menuName = "EnemyStatsReference")]
public class EnemyStatsReference : ScriptableObject
{
    public EnemyStats _enemyStats { get => _enemyStats; set => _enemyStats = value; }

    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;

    public float damage;
    public float stealthDetection;
    public bool isDead = false;
}
