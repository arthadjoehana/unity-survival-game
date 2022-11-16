using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseLifeSpan : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

}
