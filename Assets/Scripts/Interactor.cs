using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    //[SerializeField] GameObject _player;
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _interactor;

    List<IInteraction> _interactions;
    

    IInteraction First => _interactions
        .Select(i => (i, distance: Vector3.Distance(i.position, transform.position)))
        .Aggregate((a, b) => a.distance < b.distance ? a : b)
        .i;

    public LayerMask AssassinationTarget;
    public Vector3 playerDirection;
    public bool assassinationTarget;

    private void Awake()
    {
        _interactions = new List<IInteraction>();
    }

    public void Update()
    {
        assassinationTarget = Physics.Raycast(_interactor.transform.position, _camera.transform.forward * 20f, out var hit, 20f, AssassinationTarget);
        if (assassinationTarget)
        {
            Debug.Log("assassination target");
            
        }
        Debug.DrawRay(_interactor.transform.position, _camera.transform.forward * 20f, Color.cyan);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IInteraction>(out var i))
        {
            _interactions.Add(i);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteraction>(out var i))
        {
            _interactions.Remove(i);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(_interactor.transform.position, _camera.transform.position + (_camera.transform.forward * 10));
    }

}
