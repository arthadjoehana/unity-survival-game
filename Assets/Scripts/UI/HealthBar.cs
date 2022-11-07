using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Slider slider;
    [SerializeField] PlayerStatsReference playerStatsReference;

    private void Start()
    {
        slider.maxValue = playerStatsReference.maxHealth;
        slider.value = playerStatsReference.maxHealth;
    }

    public void UpdateHealthBar(PlayerHealth playerHealth)
    {
        //healthText.text = $"Health : {playerHealth.CurrentHealth}";
        slider.value = playerHealth.CurrentHealth;
    }
}
