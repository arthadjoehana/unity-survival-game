using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] PlayerMovement _playerMovement;
    //[SerializeField] Collider _collider;

    public void OnTriggerEnter(Collider player)
    {
        if (_playerMovement.isCrouched)
        {
            _playerMovement.isHidden = true;
        }
        else
        {
            _playerMovement.isHidden = false;
        }
    }

    public void OnTriggerStay(Collider player)
    {
        if (_playerMovement.isCrouched)
        {
            _playerMovement.isHidden = true;
        }
        else
        {
            _playerMovement.isHidden = false;
        }
    }

    public void OnTriggerExit(Collider player)
    {
        _playerMovement.isHidden = false;
    }
}
