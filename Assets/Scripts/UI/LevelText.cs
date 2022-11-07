using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] PlayerStatsReference _playerStatsReference;
    [SerializeField] PlayerLevel _playerLevel;

    private void Start()
    {
        levelText.text = $"LV.{_playerStatsReference.level}";
    }

    public void UpdateLevelText(PlayerLevel _playerLevel)
    {
        levelText.text = $"LV.{_playerLevel.playerLevel}";
    }
}
