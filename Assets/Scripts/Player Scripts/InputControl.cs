using UnityEngine;
using UnityEngine.InputSystem;



public class InputControl : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool interact;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;


    public void OnMove(InputValue value)
    {
        Move(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            Look(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        Jump(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        Sprint(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        Crouch(value.isPressed);
    }



    public void Move(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void Look(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void Jump(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void Sprint(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void Crouch(bool newCrouchState)
    {
        crouch = newCrouchState;
    }

    public void Interact(bool newInteractState)
    {
        interact = newInteractState;
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