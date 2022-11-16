using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    List<IInteraction> _interactions;
    

    IInteraction First => _interactions
        .Select(i => (i, distance: Vector3.Distance(i.position, transform.position)))
        .Aggregate((a, b) => a.distance < b.distance ? a : b)
        .i;

    private void Awake()
    {
        _interactions = new List<IInteraction>();
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

}
