using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_HurtBox : MonoBehaviour, IHurtbox
{
    [SerializeField] private bool active = true;
    [SerializeField] private GameObject _owner = null;
    private IHurtResponder _hurtResponder;


    public bool Active => throw new System.NotImplementedException();

    public GameObject Owner => throw new System.NotImplementedException();

    public Transform Transform => throw new System.NotImplementedException();

    public IHurtResponder HurtResponder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public bool CheckHit(HitData data)
    {
        if (_hurtResponder == null)
            Debug.Log("No Responder");

        return true;
    }

    
}
