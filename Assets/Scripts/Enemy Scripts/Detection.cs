using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Detection : MonoBehaviour
{
    //ref
    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] GameObject _vision;
    [SerializeField] EnemyAI _enemyAI;
    [SerializeField] EnemyUI _enemyUI;

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _playerHead;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerHealth _playerHealth;

    //detection values
    [SerializeField] public float stealthDetection;
    [SerializeField] public float NoiseDetectionRadius;
    [SerializeField] public float visionRadius;
    [Range(0, 360)]
    [SerializeField] public float visionAngle;

    //combat values
    [SerializeField] public float combatRadius;
    [SerializeField] public float attackRadius;
    [Range(0, 360)]
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackDelay;

    //other values
    public Vector3 playerDirection;
    public float playerAngle;

    public enum AWARENESS
    {
        OBLIVIOUS,
        SUSPICIOUS,
        ALERTED,
    }
    public AWARENESS currentAwareness = AWARENESS.OBLIVIOUS;

    //player detection
    public float reactionTime;
    public bool reaction;
    public bool processingReaction; //the processing time of the enemy reacting to the player's presence
    public bool isAlert;
    public LayerMask Obstacles;
    public bool visionObstructed;
    public float distance; //distance between the enemy and the player
    public bool playerIsAlive;
    public bool playerHeadVisibility;
    public bool playerisHidden;
    public bool playerInNoiseDetectionRange;
    public bool playerInCombatRange;
    public bool playerInAttackRange;
    public bool playerInFront;
    public bool playerInFieldOfView;
    public bool playerDetected;

    
    public bool stealthCheck; //if passed, the enemy detects the player

    private void Start()
    {
        isAlert = false;
    }
    private void Update()
    {
        reactionTime = isAlert ? 1 : 3;

        distance = Vector3.Distance(_player.transform.position, transform.position);

        playerIsAlive = !_playerMovement.isDead;
        playerisHidden = _playerMovement.isHidden;

        playerDirection = _playerHead.transform.position - _vision.transform.position;
        playerAngle = Vector3.Angle(playerDirection, _vision.transform.forward);

        playerInNoiseDetectionRange = distance <= NoiseDetectionRadius && distance > combatRadius;
        playerInCombatRange = distance <= combatRadius && distance > attackRadius;
        playerInAttackRange = distance <= attackRadius;

        playerInFront = playerInAttackRange && playerAngle < attackAngle && playerAngle > -attackAngle;

        playerInFieldOfView =
            distance < visionRadius &&
            playerAngle < visionAngle &&
            playerAngle > -visionAngle;

        visionObstructed = Physics.Raycast(_vision.transform.position, playerDirection, out var hit, playerDirection.magnitude, Obstacles);

        stealthCheck = stealthDetection > _playerMovement.totalStealth;

        DetectPlayer();
    }

    public void DetectPlayer()
    {
        if (playerIsAlive)
        {
            if (playerInFieldOfView && !visionObstructed && !playerisHidden)//sight based detection
            {
                if (!reaction)
                {
                    StartCoroutine(Reaction());
                }

                if (playerInAttackRange || playerInCombatRange || playerInNoiseDetectionRange)
                {
                    //_playerMovement.ChangeStatus(PlayerMovement.STATUS.COMPROMISED);
                    isAlert = true;
                    ChangeAwareness(AWARENESS.ALERTED);
                }
            }
            else if (playerInNoiseDetectionRange && stealthCheck)//hearing based detection
            {
                if (!reaction)
                {
                    StartCoroutine(Reaction());
                }
            }
            else if (playerInCombatRange || playerInAttackRange)
            {
                if (!reaction)
                {
                    StartCoroutine(Reaction());
                }
            }
        }
        else
        {
            playerDetected = false;
        }

        if (_playerMovement.isHidden)
        {
            playerDetected = false;
        }
    }

    public void ChangeAwareness(AWARENESS newState)
    {
        currentAwareness = newState;
    }

    IEnumerator Reaction()
    {
        reaction = true;
        processingReaction = true;
        yield return new WaitForSeconds(reactionTime);
        reaction = false;
        processingReaction = false;
        playerDetected = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, NoiseDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, visionAngle, 0) * transform.forward * visionRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -visionAngle, 0) * transform.forward * visionRadius));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combatRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, attackAngle, 0) * transform.forward * attackRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -attackAngle, 0) * transform.forward * attackRadius));

        if (playerDetected)
        {
            Gizmos.color = Color.green;
        }
        else if (processingReaction)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(_vision.transform.position, _player.transform.position);
        Gizmos.DrawLine(_vision.transform.position, _playerHead.transform.position);
    }
}
