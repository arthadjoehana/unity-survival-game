using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // references
    [SerializeField] PlayerStatsReference _playerStatsRef;
    [SerializeField] GameObject _playerHead;
    [SerializeField] Animator _animator;
    [SerializeField] CharacterController _controller;
    [SerializeField] InputControl _input;
    [SerializeField] CameraMovement _cameraMovement;
    [SerializeField] GameObject _mainCamera;
    [SerializeField] EnemyAI _enemyAI;
    [SerializeField] GameObject _playerCameraRoot;
    [SerializeField] AnimationScript _animation;
    [SerializeField] GameObject _sheathedWeapon;
    [SerializeField] GameObject _unsheathedWeapon;
    [SerializeField] public GameObject _noiseSpot;

    public float walkSpeed = 2.0f;
    public float sprintSpeed = 6.0f;
    public float sneakSpeed = 1.0f;
    public float targetSpeed;
    [Range(0.0f, 0.3f)]
    public float rotationSpeed = 0.12f;
    private float rotationVelocity;
    public float speedChangeRate = 10.0f;
    public float speed;
    private float animationBlend;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    public float attackCoolDown = 3.0f;
    public float attackRadius;
    public float attackAngle = 30.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] 
    public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    [Space(10)]
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    public LayerMask Ground;

    //States
    public bool isMoving;
    public bool isCrouched;
    public bool isHidden;
    public bool isRunning;
    public bool isFighting;
    public bool isAttacking;
    public bool isDefending;
    public bool isDead;

    public bool canMove;
    public bool canSprint;
    public bool canCrouch;
    public bool canFight;
    public bool canAttack;

    public bool attackHasStarted;
    public bool attack1HasStarted;
    public bool attack2HasStarted;
    public bool attack3HasStarted;

    public float totalStealth;
    public float baseStealth;
    public float crouchStealth;
    public float noise;
    public bool noiseGenerated;
    public Vector3 noiseLocation;

    public enum STATE
    {
        DEFAULT, 
        CROUCH, 
        SPRINT, 
        COMBAT, 
        ATTACK,
        ATTACK1,
        ATTACK2,
        ATTACK3,
        DEFEND, 
        DAMAGED, 
        DEAD
    }
    public STATE currentState = STATE.DEFAULT;

    public enum STATUS
    {
        ANONYMOUS, //enemies are not aware of the player
        SUSPICIOUS, //enemies will be aware of the player if they draw attention
        COMPROMISED //enemies are aware of the player
    }
    public STATUS currentStatus = STATUS.ANONYMOUS;


    private void Awake()
    {
        _playerStatsRef.PlayerMovement = this;
    }

    private void Start()
    {
        baseStealth = _playerStatsRef.baseStealth;
        crouchStealth = _playerStatsRef.crouchStealth;

        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        isDead = false;
    }

    private void Update()
    {
        switch (currentState)
        {
            case STATE.DEFAULT:
                canMove = true;
                canSprint = true;
                canCrouch = true;
                canFight = true;
                canAttack = false;
                break;
            case STATE.SPRINT:
                canMove = true;
                canSprint = true;
                canCrouch = true;
                canFight = true;
                canAttack = false;
                break;
            case STATE.CROUCH:
                canMove = true;
                canSprint = true;
                canCrouch = true;
                canFight = true;
                canAttack = false;
                break;
            case STATE.COMBAT:
                canMove = true;
                canSprint = false;
                canCrouch = false;
                canFight = true;
                canAttack = true;
                break;
            case STATE.ATTACK1:

                break;
            case STATE.ATTACK2:

                break;
            case STATE.ATTACK3:

                break;
            case STATE.DEAD:
                canMove = false;
                canSprint = false;
                canCrouch = false;
                canFight = false;
                canAttack = false;
                break;
            default:

                break;
        }

        totalStealth = baseStealth - noise;

        GroundedCheck();
        Action();
        JumpAndGravity();
        Death();

    }


    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, Ground,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animation.grounded, grounded);
    }

    private void Action()
    {
        targetSpeed = walkSpeed;
        isMoving = _input.move != Vector2.zero;
        isFighting = _input.combat;
        isCrouched = _input.crouch;
        isRunning = _input.sprint;
        isAttacking = _input.attack;

        //idle
        if (!isMoving)
        {
            targetSpeed = 0;
        }

        //default
        if (!isRunning && !isCrouched && !isFighting)
        {
            ChangeState(STATE.DEFAULT);
        }

        //move
        if (canMove && isMoving)
        {   
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;
        }
        Vector3 direction = transform.right * _input.move.x + transform.forward * _input.move.y;
        _controller.Move(direction.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        //sprint
        if (canSprint && isRunning && !isFighting)
        {
            if (isMoving)
            {
                if(!noiseGenerated)
                {
                    StartCoroutine(CreateNoise());
                    Debug.Log("noise created");
                }
                ChangeState(STATE.SPRINT);
                targetSpeed = sprintSpeed;
                _input.crouch = false;
            }
            else
            {
                ChangeState(STATE.DEFAULT);
            }
        }

        //crouch

        if (canCrouch && isCrouched)
        {
            ChangeState(STATE.CROUCH);
            _playerCameraRoot.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);
            //_input.combat = false;
            targetSpeed = isMoving ? sneakSpeed : 0;
            _playerHead.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        }
        else
        {
            _playerCameraRoot.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            _playerHead.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        }
        
        //combat
        if (canFight && isFighting)
        {
            _unsheathedWeapon.SetActive(true);
            ChangeState(STATE.COMBAT);
            _input.crouch = false;

            if(isRunning)
            {
                if (isMoving)
                {
                    targetSpeed = sprintSpeed;
                    canAttack = false;
                    isAttacking = false;
                    _input.attack = false;
                }
                else
                {
                    targetSpeed = 0;
                    canAttack = true;
                }

            }
        }
        else
        {
            _unsheathedWeapon.SetActive(false);
        }

        //attack
        if (canAttack && isAttacking && !attack1HasStarted)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;
            StartCoroutine(Attack1());
        }

        //smooth speed change
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        //animations
        _animator.SetFloat(_animation.speed, animationBlend);
        _animator.SetFloat(_animation.xAxis, _input.move.x);
        _animator.SetFloat(_animation.yAxis, _input.move.y);
        _animator.SetFloat(_animation.motionSpeed, inputMagnitude);
        if (canSprint && isRunning && isMoving) _animator.SetBool(_animation.sprint, true);
        else _animator.SetBool(_animation.sprint, false);
        if (canCrouch && isCrouched) _animator.SetBool(_animation.crouch, true);
        else _animator.SetBool(_animation.crouch, false);
        if (canFight && isFighting)
        {
            if (isRunning && isMoving)
            {
                _animator.SetBool(_animation.sprint, true);
                _animator.SetBool(_animation.combat, false);
            }
            else
            {
                _animator.SetBool(_animation.combat, true);
                _animator.SetBool(_animation.sprint, false);
            }
        }
        else
        {
            _animator.SetBool(_animation.combat, false);
        }
    }

    IEnumerator Attack1()
    {
        attack1HasStarted = true;
        canAttack = false;
        ChangeState(STATE.ATTACK1);

        _animator.SetTrigger(_animation.attack1);
        Debug.Log("attack 1");

        yield return new WaitForSeconds(attackCoolDown);

        attack1HasStarted = false;
        canAttack = true;
        ChangeState(STATE.COMBAT);
    }

    IEnumerator CreateNoise()
    {
        noiseGenerated = true;
        Instantiate(_noiseSpot, transform.position, transform.rotation);
        noiseLocation = _noiseSpot.transform.position;
        yield return new WaitForSeconds(0.2f);
        noiseGenerated = false;
    }
    

    private void JumpAndGravity()
    {
        if (grounded)
        {
            fallTimeoutDelta = fallTimeout;
            _animator.SetBool(_animation.jump, false);
            _animator.SetBool(_animation.freeFall, false);
            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
            if (_input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _animator.SetBool(_animation.jump, true);
            }
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animation.freeFall, true);
            }
            _input.jump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void Death()
    {
        if (_playerStatsRef.currentHealth <= 0)
            if (!isDead)
            {
                ChangeState(STATE.DEAD);
                _animator.SetTrigger(_animation.death);
                isDead = true;
            }
    }

    public void ChangeState(STATE newState)
    {
        currentState = newState;
    }

    public void ChangeStatus(STATUS newStatus)
    {
        currentStatus = newStatus;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}
