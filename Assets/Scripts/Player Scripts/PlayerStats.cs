using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] PlayerStatsReference _playerStatsRef;



    public float stealth;
    public float crouchStealth;
   

    

    void Awake()
    {
    
    }

    public void Start()
    {
        _playerStatsRef.PlayerStats = this;
    }

    void Update()
    {

    }

    public void TakeDamage(float damage, bool overTime = false)
    {
        if(overTime)
        {
            _playerStatsRef.currentHealth -= damage * Time.deltaTime;
        } 
        else
        {
            _playerStatsRef.currentHealth -= damage * (1 - (_playerStatsRef.currentArmorPoints / 100));
        }

        if(_playerStatsRef.currentHealth <= 0 && !_playerStatsRef.isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died !");
        _playerStatsRef.isDead = true;

        // Bloque le mouvement du joueur + mode inspection
        

        // On bloque la diminution des barres de faim et soif
        //playerStats.hungerDecreaseRate = 0;
        //playerStats.thirstDecreaseRate = 0;

        //animator.SetTrigger("Die");
    }


 

}
