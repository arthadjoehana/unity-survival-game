using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private PlayerStatsReference playerStats;

    [Header("Other elements references")]
    [SerializeField] private Animator animator;

    [SerializeField] private playerMovement playerMovementScript;

    //[SerializeField] private AimBehaviourBasic playerAimScript;


    public float stealth;
    public float crouchStealth;
   

    

    void Awake()
    {
        playerStats.currentHealth = playerStats.maxHealth;
        playerStats.currentStamina = playerStats.maxStamina;
        stealth = playerStats.stealth;
        crouchStealth = playerStats.crouchStealth;
        //playerStats.currentHunger = playerStats.maxHunger;
        //playerStats.currentThirst = playerStats.maxThirst;
        //playerStats.currentSleep = playerStats.maxSleep;

    }

    public void Start()
    {

    }

    void Update()
    {
        //UpdateHungerAndThirstBarsFill();

       /* if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50f);
        }*/
    }

    public void TakeDamage(float damage, bool overTime = false)
    {
        if(overTime)
        {
            playerStats.currentHealth -= damage * Time.deltaTime;
        } 
        else
        {
            playerStats.currentHealth -= damage * (1 - (playerStats.currentArmorPoints / 100));
        }

        if(playerStats.currentHealth <= 0 && !playerStats.isDead)
        {
            Die();
        }
        
        UpdateHealthBarFill();
    }

    private void Die()
    {
        Debug.Log("Player died !");
        playerStats.isDead = true;

        // Bloque le mouvement du joueur + mode inspection
        

        // On bloque la diminution des barres de faim et soif
        //playerStats.hungerDecreaseRate = 0;
        //playerStats.thirstDecreaseRate = 0;

        //animator.SetTrigger("Die");
    }

    /*public void ConsumeItem(float health, float hunger, float thirst)
    {
        playerStats.currentHealth += health;

        if(playerStats.currentHealth > playerStats.maxHealth)
        {
            playerStats.currentHealth = playerStats.maxHealth;
        }

        playerStats.currentHunger += hunger;

        if(playerStats.currentHunger > playerStats.maxHunger)
        {
            playerStats.currentHunger = playerStats.maxHunger;
        }

        playerStats.currentThirst += thirst;

        if(playerStats.currentThirst > playerStats.maxThirst)
        {
            playerStats.currentThirst = playerStats.maxThirst;
        }

        UpdateHealthBarFill();
    }*/

    void UpdateHealthBarFill()
    {
        playerStats.healthBarFill.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
    }

    /*void UpdateHungerAndThirstBarsFill()
    {
        // Diminue la faim / soif au fil du temps
        playerStats.currentHunger -= playerStats.hungerDecreaseRate * Time.deltaTime;
        playerStats.currentThirst -= playerStats.thirstDecreaseRate * Time.deltaTime;

        // On empêche de passer dans le négatif
        playerStats.currentHunger = playerStats.currentHunger < 0 ? 0 : playerStats.currentHunger;
        playerStats.currentThirst = playerStats.currentThirst < 0 ? 0 : playerStats.currentThirst;

        // Mettre à jour les visuels
        playerStats.hungerBarFill.fillAmount = playerStats.currentHunger / playerStats.maxHunger;
        playerStats.thirstBarFill.fillAmount = playerStats.currentThirst / playerStats.maxThirst;

        // Si la barre de faim et/ou soif est à zéro -> Le joueur prend des dégâts (x2 si les deux barres sont à zéro)
        if(playerStats.currentHunger <= 0 || playerStats.currentThirst <= 0)
        {
            TakeDamage((playerStats.currentHunger <= 0 && playerStats.currentThirst <= 0 ? playerStats.healthDecreaseRateForHungerAndThirst * 2 : playerStats.healthDecreaseRateForHungerAndThirst), true);
        }
    }*/
}

internal class playerMovement
{
}