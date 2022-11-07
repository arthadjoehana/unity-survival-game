using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]

    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] GameObject _player;
    [SerializeField] Animator _animator;
    [SerializeField] AnimationScript _animation;
    [SerializeField] Collider _collider;
    [SerializeField] EnemyHealth _enemyHealth;

    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] PlayerLevel _playerLevel;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerHealth _playerHealth;

    //movement values
    [SerializeField] private float walkSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float rotationSpeed;

    //detection values
    [SerializeField] private float stealthDetection;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float visionRadius;
    [Range(0, 360)]
    [SerializeField] private float visionAngle;

    //combat values
    [SerializeField] private float combatRadius;
    [SerializeField] private float attackRadius;
    [Range(0, 360)]
    [SerializeField] private float attackAngle;
    [SerializeField] public float attackDelay;
    
    //other values
    private Vector3 playerDirection;
    private float playerAngle;

    //wandering settings
    [SerializeField] private float wanderingWaitTimeMin;
    [SerializeField] private float wanderingWaitTimeMax;
    [SerializeField] private float wanderingDistanceMin;
    [SerializeField] private float wanderingDistanceMax;

    public enum STATE
    {
        BEHAVIOUR, 
        CHASE, 
        COMBAT,
        ATTACK, 
        RETURNTOPOSITION, 
        WANDER, 
        PATROL, 
        SCRIPTED, 
        DEAD
    }
    public STATE currentState = STATE.BEHAVIOUR;

    public enum BEHAVIOUR
    {
        IDLE, WANDER, PATROL, SCRIPTED
    }
    public BEHAVIOUR Behaviour;

    //movement
    private Vector3 originalPosition;
    private bool hasDestination;
    private bool noDirection;

    //states
    public bool isAttacking;   
    private bool isDead;

    //player detection
    private bool obstacle;
    private bool playerInDetectionRange;
    private bool playerInCombatRange;
    private bool playerInAttackRange;
    private bool playerInFront;
    private bool playerInSight;
    private bool playerDetected;
    private bool playerStealthCheck;
    private bool playerIsAlive;

    private void Start()
    {
        originalPosition = transform.position;
        playerIsAlive = true;
        isDead = false;
    }

    void Update()
    {
        playerIsAlive = _playerHealth.playerHealth > 0;
        playerDirection = _player.transform.position - transform.position;
        playerAngle = Vector3.Angle(playerDirection, transform.forward);
        playerInDetectionRange = Vector3.Distance(_player.transform.position, transform.position) < detectionRadius;
        playerInCombatRange = Vector3.Distance(_player.transform.position, transform.position) < combatRadius;
        playerInAttackRange = Vector3.Distance(_player.transform.position, transform.position) < attackRadius;
        playerInFront = playerAngle < attackAngle;
        playerInSight = playerDirection.magnitude < visionRadius && playerAngle < visionAngle;
        playerStealthCheck =  stealthDetection > _playerMovement.totalStealth;
        noDirection = _agent.remainingDistance < 0.75f && !hasDestination;

        switch (currentState)
        {
            case STATE.BEHAVIOUR:
                ChangeStateBasedOnConf();
                break;

            case STATE.CHASE:
                _agent.speed = chaseSpeed;
                _agent.isStopped = false;
                if (playerIsAlive)
                {
                    if (playerDetected || playerInDetectionRange)
                    {
                        _agent.SetDestination(_player.transform.position);
                    }

                    if (playerInCombatRange && playerDetected)
                    {    
                        ChangeState(STATE.COMBAT);
                    }
                }
                else
                {
                    ChangeStateBasedOnConf();
                }

                break;

            case STATE.COMBAT:
                if (playerIsAlive)
                {
                    if (playerInCombatRange)
                    {
                        _animator.SetBool(_animation.combat, true);
                        _agent.speed = walkSpeed;

                        if (playerInAttackRange)
                        {
                            _agent.isStopped = true;
                            if (!isAttacking && playerInFront)
                            {
                                StartCoroutine(AttackPlayer());
                            }
                            else
                            {
                                Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                            }
                        }
                        else if (!isAttacking)
                        {
                            _agent.isStopped = false;
                            _agent.SetDestination(_player.transform.position);
                            Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        _animator.SetBool(_animation.combat, false);
                        ChangeState(STATE.CHASE);
                    }
                }
                else
                {
                    _animator.SetBool(_animation.combat, false);
                    ChangeStateBasedOnConf();
                }
                break;

            case STATE.RETURNTOPOSITION:
                _agent.SetDestination(originalPosition);
                break;

            case STATE.WANDER:
                _agent.speed = walkSpeed;
                _agent.isStopped = false;
                if (noDirection)
                {
                    StartCoroutine(GetNewDestination());
                }
                if (playerDetected && playerIsAlive)
                {
                    ChangeState(STATE.CHASE);
                }
                break;

            case STATE.PATROL:
                break;

            case STATE.SCRIPTED:
                break;

            case STATE.ATTACK:

                break;
            case STATE.DEAD:

                break;
        }

        DetectPlayer();
        Die();

        _animator.SetFloat(_animation.speed, _agent.velocity.magnitude);

    }

    private void ChangeStateBasedOnConf()
    {
        switch (Behaviour)
        {
            case BEHAVIOUR.IDLE:
                ChangeState(STATE.RETURNTOPOSITION);
                break;
            case BEHAVIOUR.WANDER:
                ChangeState(STATE.WANDER);
                break;
            case BEHAVIOUR.PATROL:
                ChangeState(STATE.PATROL);
                break;
            case BEHAVIOUR.SCRIPTED:
                ChangeState(STATE.RETURNTOPOSITION);
                break;
            default:
                break;
        }
    }

    void DetectPlayer()
    {
        obstacle = Physics.Raycast(transform.position, playerDirection, out var hit, playerDirection.magnitude, LayerMask.GetMask("Obstacle"));

        if (playerIsAlive)
        {
            if (obstacle)
            {
                Debug.Log(hit.collider.name);
                Debug.DrawRay(transform.position, playerDirection, Color.red, 0.5f);
                playerDetected = false;
                return;
            }

            if (playerInSight)
            {
                Debug.DrawRay(transform.position, playerDirection, Color.green, 0.5f);
                playerDetected = true;
            }
            if (playerInAttackRange)
            {
                Debug.DrawRay(transform.position, playerDirection, Color.green, 0.5f);
                playerDetected = true;
            }
            else
            {
                if (playerInDetectionRange && playerStealthCheck)
                {
                    if (obstacle)
                    {
                        Debug.Log(hit.collider.name);
                        Debug.DrawRay(transform.position, playerDirection, Color.red, 0.5f);
                        playerDetected = false;
                        return;
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, playerDirection, Color.green, 0.5f);
                        playerDetected = true;
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, playerDirection, Color.red, 0.5f);
                    playerDetected = false;
                }
            }
        }
        else
        {
            playerDetected = false;
        }

    }

    IEnumerator GetNewDestination()
    {
        hasDestination = true;
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));

        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin, wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1), 0f, Random.Range(-1f, 1f)).normalized;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        hasDestination = false;
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        _agent.isStopped = true;

        _animator.SetTrigger(_animation.attack);
        
        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
        _agent.isStopped = false;
    }

    void Die()
    {
        if (_enemyHealth.currentHealth <= 0)
            if (!isDead)
            {
                ChangeState(STATE.DEAD);
                _animator.SetTrigger(_animation.death);
                isDead = true;
                _collider.enabled = false;
                _playerStatsRef.exp += _enemyStatsRef.exp;
                _playerLevel.UpdateExp();
            }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, visionAngle, 0) * transform.forward  * visionRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -visionAngle, 0) * transform.forward * visionRadius));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combatRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, attackAngle, 0) * transform.forward * attackRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -attackAngle, 0) * transform.forward * attackRadius));
    }

    
    public void ChangeState(STATE newState)
    {
        currentState = newState;
    }
}
