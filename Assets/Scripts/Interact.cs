using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private float interactRange = 2.6f;
    private InputControl _input;

    public InteractBehaviour playerInteractBehaviour;

    [SerializeField]
    private GameObject interactText;

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        _input = GetComponent<InputControl>();
    }
    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask))
        {
            interactText.SetActive(true);

            if (_input.interact)
            {
                if (hit.transform.CompareTag("Item"))
                {
                    playerInteractBehaviour.DoPickup(hit.transform.gameObject.GetComponent<Item>());
                }

                if (hit.transform.CompareTag("Harvestable"))
                {
                    playerInteractBehaviour.DoHarvest(hit.transform.gameObject.GetComponent<Harvestable>());
                }

            }

        }
        else
        {
            interactText.SetActive(false);
        }
    }
}
