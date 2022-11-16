using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponCollider : MonoBehaviour
{
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Collider _collider;

    public void OnTriggerEnter(Collider hitbox)
    {
        var enemy = hitbox.GetComponentInParent<EnemyHealth>();
        if (_playerMovement.attack1HasStarted && enemy != null)
        {
            StartCoroutine(Hit(enemy));
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

    private void OnDisable()
    {
        StopAllCoroutines();
        _collider.enabled = true;
    }
}
