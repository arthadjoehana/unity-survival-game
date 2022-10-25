using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    public void UpdateHealthText(PlayerHealth playerHealth)
    {
        healthText.text = $"Health : {playerHealth.Health}";
    }
}
