using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _interactor;
    [SerializeField] InputControl _input;

    List<IInteraction> _interactions;
    private bool assassinate;

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
            Debug.DrawRay(_interactor.transform.position, _camera.transform.forward * 20f, Color.cyan);
            //Debug.Log(hit.collider.name);
            var target = hit.collider.GetComponentInParent<EnemyHealth>();
            var detection = hit.collider.GetComponentInParent<Detection>();
            if (_input.assassinate && !assassinate && target != null && detection != null && !detection.isAlert)
            {
                Debug.Log("target assassinated");
                StartCoroutine(Assassinate(target));   
            }
        }
        else
        {
            Debug.DrawRay(_interactor.transform.position, _camera.transform.forward * 20f, Color.magenta);
        }
    }

    IEnumerator Assassinate(EnemyHealth enemyHealth)
    {
        assassinate = true;
        enemyHealth.Assassinated();
        Debug.Log("isdead0");
        yield return new WaitForSeconds(1f);
        assassinate = false;
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
