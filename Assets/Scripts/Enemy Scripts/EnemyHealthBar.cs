using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] EnemyHealth _enemyHealth;
    [SerializeField] Camera _camera;

    private void Start()
    {
        _slider.maxValue = _enemyHealth.maxHealth;
        _slider.value = _enemyHealth.maxHealth;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
    }

    public void UpdateHealthBar(EnemyHealth enemyHealth)
    {
        _slider.value = enemyHealth.CurrentHealth;
        if (enemyHealth.CurrentHealth <= 0)
        {
            Destroy(_slider.gameObject);
        }
    }
}
