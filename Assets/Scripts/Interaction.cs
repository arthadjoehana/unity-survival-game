using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteraction
{
    Vector3 position { get; }
    void Interact();
}

public class Interaction : MonoBehaviour, IInteraction
{
    public Vector3 position { get => transform.position; }
    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}
