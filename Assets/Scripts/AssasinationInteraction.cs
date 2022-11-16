using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssassinationTarget
{
    Vector3 position { get; }
    void Assassinate();
}

public class AssasinationInteraction : MonoBehaviour, IAssassinationTarget
{
    public Vector3 position { get => transform.position; }
    public void Assassinate()
    {
        Debug.Log("assasinated !");
    }
}
