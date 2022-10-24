using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;
using ScriptableObjectArchitecture;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement stats")]
    public float MoveSpeed = 3.0f;
    public float SprintSpeed = 6.0f;
    public float SneakSpeed = 1.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSpeed = 0.12f;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask Ground;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float speed;
    private float animationBlend;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDCrouched;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    // references

    [SerializeField] PlayerStatsReference _playerStatsRef;

    private PlayerInput _playerInput;
    private Animator _animator;
    private CharacterController _controller;
    private InputControl _input;
    private GameObject _mainCamera;
    private PlayerStats _playerStats;

    //[SerializeField] private CinemachineVirtualCamera _vCam;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    //States
    public bool isCrouched;
    public bool isDead;

    public bool canMove;
    public bool canSprint;
    public bool canCrouch;
    public bool canAttack;

    public float totalStealth;
    public float baseStealth;
    public float crouchStealth;
    public float noise;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }

    public enum STATE
    {
        STAND, WALK, CROUCH, SPRINT, ATTACK, DAMAGED, JUMP, FALL, DEAD
    }
    public STATE currState = STATE.STAND;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        _playerStatsRef.PlayerMovement = this;

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputControl>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStats>();

        AssignAnimationIDs();
    }

    private void Start()
    {
        baseStealth = _playerStatsRef.baseStealth;
        crouchStealth = _playerStatsRef.crouchStealth;

        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;

        canMove = true;
        canSprint = true;
        canCrouch = true;
    }

    private void Update()
    {
        totalStealth = baseStealth - noise;
        

        if (isCrouched)
        {
            totalStealth = baseStealth + crouchStealth;
        }
        else
        {
            totalStealth = baseStealth;
        }

        GroundedCheck();
        Move();
        JumpAndGravity();

        switch (currState)
        {
            case STATE.STAND:
                noise = 0f;
                if(_input.move != Vector2.zero)
                {
                    ChangeState(STATE.WALK);
                }
                if (_input.crouch)
                {
                    ChangeState(STATE.CROUCH);
                }
                
                canMove = true;
                canSprint = true;
                canCrouch = true;
                break;

            case STATE.WALK:
                if (_input.move != Vector2.zero)
                {
                    noise = 10f;
                    if (_input.sprint)
                    {
                        ChangeState(STATE.SPRINT);
                    }
                }
                else
                {
                    ChangeState(STATE.STAND);
                }
                if (_input.crouch)
                {
                    ChangeState(STATE.CROUCH);
                }
                canMove = true;
                canSprint = true;
                canCrouch = true;
                break;

            case STATE.CROUCH:
                noise = 0f;
                if (!_input.crouch)
                {
                    ChangeState(STATE.STAND);
                }
                canMove = true;
                canSprint = false;
                canCrouch = true;
                break;

            case STATE.SPRINT:
                if (_input.move != Vector2.zero)
                {
                    noise = 20f;
                }
                if (!_input.sprint)
                {
                    ChangeState(STATE.STAND);
                }
                canMove = true;
                canSprint = true;
                canCrouch = false;
                break;

            case STATE.ATTACK:


                break;

            case STATE.DEAD:


                break;
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDCrouched = Animator.StringToHash("Crouch");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, Ground,
            QueryTriggerInteraction.Ignore);

        if (_hasAnimator) _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        float targetSpeed = isCrouched? SneakSpeed : MoveSpeed;
        // camView = _vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        if (_input.sprint && canSprint)
        {
            targetSpeed = SprintSpeed;
            canCrouch = false;
        }

        if (_input.crouch && canCrouch)
        {
            isCrouched = true;
            _animator.SetBool(_animIDCrouched, true);
            //camView.ShoulderOffset.y = -0.5f ;
        }
        else
        {
            isCrouched = false;
            _animator.SetBool(_animIDCrouched, false);
            //camView.ShoulderOffset.y = 0f;
        }

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        if (_input.move != Vector2.zero)
        {
            Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, RotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;

            _controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, animationBlend);
            _animator.SetFloat("X axis", _input.move.x);
            _animator.SetFloat("Y axis", _input.move.y);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
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

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.STAND:

                break;
            case STATE.CROUCH:

                break;
        }
        switch (newState)
        {
            case STATE.STAND:

                break;
            case STATE.CROUCH:

                break;
        }
        currState = newState;
    }
}
