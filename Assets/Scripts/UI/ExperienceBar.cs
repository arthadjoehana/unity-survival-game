using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] PlayerStatsReference _playerStatsReference;
    [SerializeField] PlayerLevel _playerLevel;
    [SerializeField] TextMeshProUGUI expNumber;

    private void Start()
    {
        slider.maxValue = _playerLevel.expTotal;
        slider.value = _playerStatsReference.exp;
        expNumber.text = $"{_playerLevel.playerExp}/{_playerLevel.expTotal}";
    }

    public void UpdateExpBar(PlayerLevel _playerLevel)
    {
        slider.maxValue = _playerLevel.expTotal;
        slider.value = _playerLevel.playerExp;
        expNumber.text = $"{_playerLevel.playerExp}/{_playerLevel.expTotal}";
    }
}
