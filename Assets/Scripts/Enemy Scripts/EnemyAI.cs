using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    //ref
    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] Animator _animator;
    [SerializeField] AnimationScript _animation;
    [SerializeField] Collider _collider;
    [SerializeField] EnemyHealth _enemyHealth;
    [SerializeField] Detection _detection;

    [SerializeField] GameObject _player;
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] PlayerLevel _playerLevel;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerHealth _playerHealth;

    //movement values
    [SerializeField] public float walkSpeed;
    [SerializeField] public float chaseSpeed;
    [SerializeField] public float rotationSpeed;

    //combat values
    [SerializeField] public float attackDelay;

    //wandering settings
    [SerializeField] private float wanderingWaitTimeMin;
    [SerializeField] private float wanderingWaitTimeMax;
    [SerializeField] private float wanderingDistanceMin;
    [SerializeField] private float wanderingDistanceMax;

    public enum STATE
    {
        BEHAVIOUR, 
        INVESTIGATE,
        SEARCH,
        CHASE, 
        COMBAT,
        ATTACK, 
        IDLE, 
        WANDER, 
        PATROL, 
        SCRIPTED, 
        DEAD
    }
    public STATE currentState = STATE.BEHAVIOUR;

    public enum BEHAVIOUR
    {
        IDLE, 
        WANDER, 
        PATROL, 
        SCRIPTED
    }
    public BEHAVIOUR Behaviour;

    //movement
    private Vector3 originalPosition;
    public Vector3 investigationDestination;
    public bool hasDestination;
    public bool hasInvestigationDestination;
    public bool noDirection;

    //states
    public bool isAttacking;   
    public bool isDead;

    private void Start()
    {
        originalPosition = transform.position;
        _detection.playerIsAlive = true;
        isDead = false;
    }

    void Update()
    {
        noDirection = _agent.remainingDistance < 0.75f && !hasDestination;
        _animator.SetFloat(_animation.speed, _agent.velocity.magnitude);

        switch (currentState)
        {
            case STATE.BEHAVIOUR:
                BehaviourState();
                break;

            case STATE.CHASE:
                Chase();
                break;

            case STATE.INVESTIGATE:
                Investigate();
                break;
            case STATE.SEARCH:
                Search();
                break;

            case STATE.COMBAT:
                Combat();
                break;

            case STATE.IDLE:
                Idle();
                break;

            case STATE.WANDER:
                Wander();
                break;

            case STATE.PATROL:
                //Patrol();
                break;

            case STATE.SCRIPTED:
                //ScriptedBehaviour();
                break;

            case STATE.ATTACK:
                StartCoroutine(AttackPlayer());
                break;

            case STATE.DEAD:
                Die();
                break;
        }

        if (!isDead)
        {
            //rotate to face the player's direction while in field of view
            if (_detection.playerInFieldOfView && !_detection.visionObstructed && !_detection.playerisHidden)
            {
                Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (_detection.playerIsAlive && !_detection.playerisHidden)
            {
                if (_detection.playerDetected)
                {
                    Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

                    if (_detection.playerInAttackRange)
                    {
                        ChangeState(STATE.ATTACK);
                    }
                    else if (_detection.playerInCombatRange)
                    {
                        ChangeState(STATE.COMBAT);
                    }
                    else
                    {
                        if (_playerMovement.currentStatus == PlayerMovement.STATUS.ANONYMOUS)
                        {
                            ChangeState(STATE.INVESTIGATE);
                        }
                        if (_playerMovement.currentStatus == PlayerMovement.STATUS.SUSPICIOUS)
                        {
                            ChangeState(STATE.CHASE);
                            _detection.isAlert = true;
                        }
                        if (_playerMovement.currentStatus == PlayerMovement.STATUS.COMPROMISED)
                        {
                            ChangeState(STATE.CHASE);
                            _detection.isAlert = true;
                        }
                    }
                }
            }
            else if (!hasInvestigationDestination)
            {
                ChangeState(STATE.SEARCH);
            }
        }

        //animations
        if (currentState == STATE.COMBAT)
        {
            _animator.SetBool(_animation.combat, true);
        }
        else
        {
            _animator.SetBool(_animation.combat, false);
        }
    }

    public void ChangeState(STATE newState)
    {
        currentState = newState;
    }

    private void BehaviourState()
    {
        switch (Behaviour)
        {
            case BEHAVIOUR.IDLE:
                ChangeState(STATE.IDLE);
                break;
            case BEHAVIOUR.WANDER:
                ChangeState(STATE.WANDER);
                break;
            case BEHAVIOUR.PATROL:
                ChangeState(STATE.PATROL);
                break;
            case BEHAVIOUR.SCRIPTED:
                ChangeState(STATE.SCRIPTED);
                break;
            default:
                break;
        }
    }

    private void Investigate()
    {
        _agent.speed = walkSpeed;
        _agent.isStopped = false;

        if (!hasInvestigationDestination)
        {
            hasInvestigationDestination = true;
            investigationDestination = _player.transform.position;
        }
        _agent.SetDestination(investigationDestination);
        if (_agent.remainingDistance < 0.75f)
        {
            hasInvestigationDestination = false;
        }
    }

    private void Search()
    {
        _agent.speed = walkSpeed;
        _agent.isStopped = false;
        if (noDirection)
        {
            StartCoroutine(GetNewDestination());
        }
        StartCoroutine(SearchTimer());
    }

    private void Chase()
    {
        _agent.speed = chaseSpeed;
        _agent.isStopped = false;
        _agent.SetDestination(_player.transform.position);
    }

    private void Combat()
    {
        _agent.speed = walkSpeed;
        _agent.isStopped = false;

        if (!isAttacking)
        {
            _agent.SetDestination(_player.transform.position);
        }
    }
    
    private void Idle()
    {
        _agent.speed = walkSpeed;
        _agent.isStopped = false;
        _agent.SetDestination(originalPosition);
    }

    private void Wander()
    {
        _agent.speed = walkSpeed;
        _agent.isStopped = false;
        if (noDirection)
        {
            StartCoroutine(GetNewDestination());
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

    IEnumerator SearchTimer()
    {
        yield return new WaitForSeconds(20);
        if (!_detection.playerDetected)
        {
            BehaviourState();
        }
    }

    IEnumerator AttackPlayer()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            _agent.isStopped = true;

            _animator.SetTrigger(_animation.attack);

            yield return new WaitForSeconds(attackDelay);

            isAttacking = false;
            _agent.isStopped = false;
        }
    }

    public void Die()
    {
        if (!isDead)
        {
            _animator.SetTrigger(_animation.death);
            isDead = true;
            _collider.enabled = false;
            _playerLevel.ExpUp(_enemyStatsRef.exp);
        }
    }
}
