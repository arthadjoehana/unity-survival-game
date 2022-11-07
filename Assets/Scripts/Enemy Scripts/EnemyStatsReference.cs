using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyStatsReference", menuName = "EnemyStatsReference")]
public class EnemyStatsReference : ScriptableObject
{
    public float maxHealth = 100f;
    public float currentHealth;

    public float damage;
    public float exp;
    public float stealthDetection;
    public bool isDead = false;
}
