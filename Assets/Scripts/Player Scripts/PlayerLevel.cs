using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] ExperienceBar _experienceBar;
    [SerializeField] LevelText _levelText;
    
    public float expTotal;
    public float oldExpTotal;
    public float newExpTotal;
    public float playerExp;
    public int playerLevel;

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

    public void ExpUp(float exp)
    {
        _playerStatsRef.exp += exp;

        oldExpTotal = expTotal;
        
        UpdateExp();

        if (playerExp >= expTotal)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        _playerStatsRef.level += 1;

        _playerStatsRef.exp -= oldExpTotal;
        newExpTotal = oldExpTotal + 100;
        expTotal = newExpTotal;

        UpdateLevel();
        UpdateExp();

        _playerStatsRef.attack += 20;
        _playerStatsRef.maxHealth += 100;
        _playerStatsRef.talentPoints += 1;
    }

    public void UpdateExp()
    {
        playerExp = _playerStatsRef.exp;
        _experienceBar.UpdateExpBar(this);
    }

    public void UpdateLevel()
    {
        playerLevel = _playerStatsRef.level;
        _levelText.UpdateLevelText(this);
    }
}
