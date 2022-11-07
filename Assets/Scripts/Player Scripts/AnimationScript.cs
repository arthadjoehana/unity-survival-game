using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using UnityEditor;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public int speed;
    public int grounded;
    public int stand;
    public int crouch;
    public int combat;
    public int sprint;
    public int sheathe;
    public int unsheathe;
    public int attack;
    public int attack1;
    public int attack2;
    public int attack3;
    public int jump;
    public int freeFall;
    public int motionSpeed;
    public int death;
    public int xAxis;
    public int yAxis;

    private void Awake()
    {
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        speed = Animator.StringToHash("Speed");
        grounded = Animator.StringToHash("Grounded");
        stand = Animator.StringToHash("Stand");
        crouch = Animator.StringToHash("Crouch");
        sprint = Animator.StringToHash("Sprint");
        combat = Animator.StringToHash("Combat");
        sheathe = Animator.StringToHash("Sheathe");
        unsheathe = Animator.StringToHash("Unsheathe");
        attack = Animator.StringToHash("Attack");
        attack1 = Animator.StringToHash("Attack 1");
        attack2 = Animator.StringToHash("Attack 2");
        attack3 = Animator.StringToHash("Attack 3");
        jump = Animator.StringToHash("Jump");
        freeFall = Animator.StringToHash("FreeFall");
        motionSpeed = Animator.StringToHash("MotionSpeed");
        death = Animator.StringToHash("Death");
        xAxis = Animator.StringToHash("X axis");
        yAxis = Animator.StringToHash("Y axis");
    }
}
