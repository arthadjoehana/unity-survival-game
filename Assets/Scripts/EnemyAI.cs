using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerStats playerStats;

    [Header("Stats")]

    [SerializeField] private float walkSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackAngle = 30.0f;
    [SerializeField] private float attackDelay;
    [SerializeField] private float damageDealt;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float detectionLevel;

    [Header("Wandering parameters")]

    [SerializeField] private float wanderingWaitTimeMin;
    [SerializeField] private float wanderingWaitTimeMax;
    [SerializeField] private float wanderingDistanceMin;
    [SerializeField] private float wanderingDistanceMax;

    public enum STATE
    {
        IDLE, WANDER, CHASE, ATTACK
    }
    public STATE currState = STATE.IDLE;

    [SerializeField] private float visionDistance = 10.0f;
    [SerializeField] private float visionAngle = 45.0f;

    [SerializeField] private bool playerDetected;

   private bool hasDestination;
   private bool isAttacking;
    
    private void Awake()
    {
        /*Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        player = playerTransform;
        playerStats = playerTransform.GetComponent<PlayerStats>();*/
    }

    private void Start()
    {
        playerDetected = false;
    }

    void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        switch (currState)
        {
            case STATE.IDLE:
                
                if (playerDetected)
                {
                    ChangeState(STATE.CHASE);
                   
                }
                else if (Random.Range(0, 100) < 10)
                {
                    ChangeState(STATE.WANDER);
                    
                }
                break;
            case STATE.WANDER:
                agent.speed = walkSpeed;
                if (agent.remainingDistance < 0.75f && !hasDestination)
                {
                    StartCoroutine(GetNewDestination());
                }
                if (playerDetected)
                {
                    ChangeState(STATE.CHASE);
                    
                }
                break;
            case STATE.CHASE:
                agent.speed = chaseSpeed;
                if (Vector3.Distance(player.transform.position, transform.position) < detectionRadius)
                {
                    agent.SetDestination(player.transform.position);
                    if (Vector3.Distance(player.transform.position, transform.position) < attackRadius && Vector3.Distance(player.transform.position, transform.position) < attackRadius && angle < attackAngle)
                    {
                        ChangeState(STATE.ATTACK);
                    }
                }
                else
                {
                    ChangeState(STATE.WANDER);
                }
                break;
            case STATE.ATTACK:
                //if player is within enemy attack radius)
                if (Vector3.Distance(player.transform.position, transform.position) < attackRadius)
                {
                    //if player is in front of enemy
                    if (angle < attackAngle)
                    {
                        if (!isAttacking)
                        {
                            StartCoroutine(AttackPlayer());
                        }
                    }
                    else
                    {
                        transform.LookAt(player.transform);
                    }
                }
                else
                {
                    ChangeState(STATE.CHASE);
                }




                break;
        }
        DetectPlayer();

        /*if (Vector3.Distance(player.position, transform.position) < detectionRadius && !playerStats.isDead)
        {
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;

            Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

            if (!isAttacking)
            {
                if (Vector3.Distance(player.position, transform.position) < attackRadius)
                {
                    StartCoroutine(AttackPlayer());
                }
                else
                {
                    agent.SetDestination(player.position);
                }
            }

        }*/
        /*else
        {
            agent.speed = walkSpeed;

            if (agent.remainingDistance < 0.75f && !hasDestination)
            {
                StartCoroutine(GetNewDestination());
            }
        }*/

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void DetectPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;

        //vision blocked by obstacles
        if (Physics.Raycast(transform.position, direction, out var hit, direction.magnitude, LayerMask.GetMask("obstacle")))
        {
            Debug.Log(hit.collider.name);
            Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
            playerDetected = false;
            return;
        }
        

        float angle = Vector3.Angle(direction, transform.forward);
        
        //cone vision
        if (direction.magnitude < visionDistance && angle < visionAngle)
        {
            Debug.DrawRay(transform.position, direction, Color.green, 0.5f);
            playerDetected = true;

        }
        else
        {
            //player is near the enemy
            if (Vector3.Distance(player.transform.position, transform.position) < detectionRadius /*&& player.stealthLevel < detectionLevel*/)
            {
                Debug.DrawRay(transform.position, direction, Color.green, 0.5f);
                playerDetected = true;
            }
            else
            {
                Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
                playerDetected = false;
            }
            
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

        //playerStats.TakeDamage(damageDealt);

        yield return new WaitForSeconds(attackDelay);
        agent.isStopped = false;
        isAttacking = false;
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
        switch (currState)
        {
            case STATE.IDLE:

                break;
            case STATE.WANDER:

                break;
            case STATE.CHASE:

                break;
            case STATE.ATTACK:

                break;
        }
        switch (newState)
        {
            case STATE.IDLE:

                break;
            case STATE.WANDER:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case STATE.CHASE:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                break;
            case STATE.ATTACK:

                break;
        }

        currState = newState;
    }
}
