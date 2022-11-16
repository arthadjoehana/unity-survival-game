using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputControl : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] Animator _animator;
    [SerializeField] AnimationScript _animation;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool attack;
    public bool canAttack;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool canCrouch;
    public bool interact;
    public bool pickpocket;
    public bool assassinate;
    public bool combat;
    public bool weapon;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void Start()
    {
        canAttack = false;
        canCrouch = true;
    }

    private void Update()
    {
        canCrouch = !sprint;
        canAttack = !sprint;
    }

    public void OnMove(InputValue inputValue)
    {
        Move(inputValue.Get<Vector2>());
    }

    public void OnLook(InputValue inputValue)
    {
        if (cursorInputForLook)
        {
            Look(inputValue.Get<Vector2>());
        }
    }
    public void OnAttack(InputValue inputValue)
    {
        if (canAttack) Attack(inputValue.isPressed);
    }

    public void OnJump(InputValue inputValue)
    {
        Jump(inputValue.isPressed);
    }

    public void OnSprint(InputValue inputValue)
    {
        Sprint(inputValue.isPressed);
    }

    public void OnCrouch(InputValue inputValue)
    {
        if (canCrouch) Crouch(inputValue.isPressed);
    }

    public void OnInteract(InputValue inputValue)
    {
        StartCoroutine(Interact());
    }

    public void OnPickpocket(InputValue inputValue)
    {
        StartCoroutine(Pickpocket());
    }

    public void OnAssassinate(InputValue inputValue)
    {
        StartCoroutine(Assassinate());
    }

    public void OnCombat(InputValue inputValue)
    {
        Combat(inputValue.isPressed);
    }

    public void Move(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void Look(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
    public void Attack(bool newAttackState)
    {
        attack = newAttackState;
        Debug.Log("attack is pressed");
    }

    public void Jump(bool newJumpState)
    {
        jump = newJumpState;
        Debug.Log("jump is pressed");
    }

    public void Sprint(bool newSprintState)
    {
        sprint = newSprintState;
        Debug.Log("sprint is pressed");
    }

    public void Crouch(bool newCrouchState)
    {
        crouch = !crouch;
        Debug.Log("crouch is pressed");
    }

    /*public void Interact(bool newInteractState)
    {
        interact = newInteractState;
        Debug.Log("interact is pressed");
    }*/

    /*public void Pickpocket(bool newPickpocketState)
    {
        pickpocket = newPickpocketState;
        Debug.Log("pickpocket is pressed");
    }*/

    /*public void Assassinate(bool newAssassinateState)
    {
        assassinate = newAssassinateState;
        Debug.Log("assassinate is pressed");
    }*/

    IEnumerator Interact()
    {
        interact = true;
        Debug.Log("interact is pressed");
        yield return new WaitForSeconds(0.5f);
        interact = false;
    }

    IEnumerator Pickpocket()
    {
        pickpocket = true;
        Debug.Log("pickpocket is pressed");
        yield return new WaitForSeconds(0.5f);
        pickpocket = false;
    }

    IEnumerator Assassinate()
    {
        assassinate = true;
        Debug.Log("assassinate is pressed");
        yield return new WaitForSeconds(0.5f);
        assassinate = false;
    }

    public void Combat(bool newCombatState)
    {
        combat = !combat;
        Debug.Log("combat is pressed");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}