using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private PlayerStatsReference _playerStatsRef;
    [SerializeField] private EnemyStatsReference _enemyStatsRef;
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Stats")]

    [SerializeField] public float damage;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackAngle = 30.0f;
    [SerializeField] private float attackDelay;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float stealthDetection;
    [SerializeField] private float visionDistance = 20.0f;
    [SerializeField] private float visionAngle = 60.0f;


    [Header("Wandering parameters")]

    [SerializeField] private float wanderingWaitTimeMin;
    [SerializeField] private float wanderingWaitTimeMax;
    [SerializeField] private float wanderingDistanceMin;
    [SerializeField] private float wanderingDistanceMax;

    public enum STATE
    {
        BEHAVIOUR, CHASE, ATTACK, RETURNTOPOSITION, WANDER, PATROL, SCRIPTED
    }
    public STATE currState = STATE.BEHAVIOUR;

    public enum BEHAVIOUR
    {
        IDLE, WANDER, PATROL, SCRIPTED
    }
    public BEHAVIOUR Behaviour;

    private Vector3 originalPosition;
    private bool hasDestination;
    private bool isAttacking;
    private bool playerIsNear;
    private bool playerInRange;
    private bool playerInFront;
    private bool playerInSight;
    private bool playerIsHit;
    private bool playerDetected;
    private bool playerStealthCheck; 
    private bool playerIsAlive;
    private bool obstacle;
    private bool noDirection;
    private Vector3 direction;
    private float angle;
    

    private void Awake()
    {
        _playerMovement = player.GetComponent<PlayerMovement>();

        damage = _enemyStatsRef.damage;


    }

    private void Start()
    {
        originalPosition = transform.position;
        playerIsAlive = true;
    }

    void Update()
    {
        playerIsAlive = _playerHealth.playerHealth > 0 ? true : false;
        direction = player.transform.position - transform.position;
        angle = Vector3.Angle(direction, transform.forward);
        playerIsNear = Vector3.Distance(player.transform.position, transform.position) < detectionRadius;
        playerInRange = Vector3.Distance(player.transform.position, transform.position) < attackRadius;
        playerInFront = angle < attackAngle;
        playerInSight = direction.magnitude < visionDistance && angle < visionAngle;
        playerStealthCheck =  stealthDetection > _playerMovement.totalStealth;
        noDirection = agent.remainingDistance < 0.75f && !hasDestination;

        switch (currState)
        {
            case STATE.BEHAVIOUR:
                ChangeStateBasedOnConf();
                break;
            case STATE.CHASE:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                if (playerDetected && playerIsAlive)
                {
                    agent.SetDestination(player.transform.position);
                    
                }
                else if(playerIsNear && playerIsAlive)
                {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    ChangeStateBasedOnConf();
                }
                if (playerInRange && playerIsAlive)
                {
                    ChangeState(STATE.ATTACK);
                }
                break;
            case STATE.RETURNTOPOSITION:
                agent.SetDestination(originalPosition);
                break;
            case STATE.WANDER:
                agent.speed = walkSpeed;
                agent.isStopped = false;
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
                if(playerIsAlive)
                {
                    if (playerInRange)
                    {
                        agent.isStopped = true;
                        if (playerInFront)
                        {
                            if (!isAttacking)
                            {
                                StartCoroutine(AttackPlayer());
                            }
                        }
                        else
                        {
                            if (!isAttacking)
                            {
                                Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                            }
                        }
                    }
                    else
                    {
                        agent.isStopped = false;
                        if (!isAttacking && playerIsAlive)
                        {
                            ChangeState(STATE.CHASE);
                        }
                    }
                }
                else
                {
                    ChangeStateBasedOnConf();
                }
                break;
        }
        DetectPlayer();

        animator.SetFloat("Speed", agent.velocity.magnitude);
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
        obstacle =  Physics.Raycast(transform.position, direction, out var hit, direction.magnitude, LayerMask.GetMask("Obstacle"));

        if (_playerHealth.playerHealth > 0)
        {
            if (obstacle)
            {
                Debug.Log(hit.collider.name);
                Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
                playerDetected = false;
                return;
            }

            if (playerInSight)
            {
                Debug.DrawRay(transform.position, direction, Color.green, 0.5f);
                playerDetected = true;
            }
            else
            {
                if (playerIsNear && playerStealthCheck)
                {
                    if (obstacle)
                    {
                        Debug.Log(hit.collider.name);
                        Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
                        playerDetected = false;
                        return;
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, direction, Color.green, 0.5f);
                        playerDetected = true;
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
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
            agent.SetDestination(hit.position);
        }
        hasDestination = false;
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true;

        animator.SetTrigger("Attack");
        _playerHealth.TakeDamage();
        /*if (playerIsHit)
        {
            _playerHealth.TakeDamage();
        }
        */

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        agent.isStopped = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    
    public void ChangeState(STATE newState)
    {
        currState = newState;
    }
}
