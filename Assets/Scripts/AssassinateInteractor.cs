using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssassinateInteractor : MonoBehaviour
{
    List<IAssassinationTarget> _assassinationTargetList;


    IAssassinationTarget First => _assassinationTargetList
        .Select(i => (i, distance: Vector3.Distance(i.position, transform.position)))
        .Aggregate((a, b) => a.distance < b.distance ? a : b).i;

    private void Awake()
    {
        _assassinationTargetList = new List<IAssassinationTarget>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAssassinationTarget>(out var i))
        {
            _assassinationTargetList.Add(i);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IAssassinationTarget>(out var i))
        {
            _assassinationTargetList.Remove(i);
        }
    }

}
