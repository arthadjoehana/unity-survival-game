using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponDamage : MonoBehaviour
{
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Collider _collider;

    public void OnTriggerEnter(Collider hitbox)
    {
        var health = hitbox.GetComponentInParent<EnemyHealth>();
        if (_playerMovement.attack1HasStarted && health != null)
        {
            StartCoroutine(Hit(health));
        }
    }

    IEnumerator Hit(EnemyHealth enemyHealth)
    {
        Debug.Log("hop");
        _collider.enabled = false;
        enemyHealth.TakeDamage(_playerStatsRef.attack);
        _audioSource.Play();
        yield return new WaitForSeconds(_playerMovement.attackCoolDown - 0.1f);
        _collider.enabled = true;
    }
}
