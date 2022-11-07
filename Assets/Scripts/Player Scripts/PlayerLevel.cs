using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] ExperienceBar _experienceBar;
    [SerializeField] LevelText _levelText;
    
    public float expTotal;
    public float playerExp;
    public int playerLevel;
    
    //public float CurrentExp { get => playerExp; }
    //public int CurrentLevel { get => playerLevel; }

    private void Awake()
    {
        _playerStatsRef.PlayerLevel = this;
        expTotal = _playerStatsRef.level * 100;
        _experienceBar.UpdateExpBar(this);
        _levelText.UpdateLevelText(this);
    }

    public void Start()
    {
        playerExp = _playerStatsRef.exp;
        playerLevel = _playerStatsRef.level;
    }

    public void Update()
    {
        if (playerExp >= expTotal)
        {
            _playerStatsRef.level += 1;
            _playerStatsRef.exp -= expTotal ;
            UpdateLevel();
            UpdateExp();
        }
    }

    public void UpdateExp()
    {
        expTotal = _playerStatsRef.level * 100;
        playerExp = _playerStatsRef.exp;
        _experienceBar.UpdateExpBar(this);
    }

    public void UpdateLevel()
    {
        playerLevel = _playerStatsRef.level;
        _levelText.UpdateLevelText(this);
    }
}
