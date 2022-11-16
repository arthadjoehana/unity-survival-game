using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitCollider : MonoBehaviour
{
    [SerializeField] EnemyAI _enemyAI;
    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Collider _collider;

    public void OnTriggerEnter(Collider hitbox)
    {
        var target = hitbox.GetComponentInParent<PlayerHealth>();
        if (_enemyAI.isAttacking && target != null)
        {
            StartCoroutine(Hit(target));
        }
    }

    IEnumerator Hit(PlayerHealth playerHealth)
    {
        Debug.Log("hop");
        _collider.enabled = false;
        playerHealth.TakeDamage(_enemyStatsRef.damage);
        _audioSource.Play();
        yield return new WaitForSeconds(_enemyAI.attackDelay - 0.1f);
        _collider.enabled = true;
    }


    private void OnDisable()
    {
        StopAllCoroutines();
        _collider.enabled = true;
    }
}

