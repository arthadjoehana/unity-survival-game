using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] public GameObject questionMark;
    [SerializeField] public GameObject exclamationMark;
    [SerializeField] Camera _camera;
    [SerializeField] Detection _detection;
    [SerializeField] EnemyAI _enemyAI;

    private void Start()
    {
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
    }

    private void Update()
    {
        if (!_enemyAI.isDead)
        {
            if (_detection.processingReaction)
            {
                questionMark.SetActive(true);
            }
            else
            {
                questionMark.SetActive(false);
            }

            if (_detection.playerDetected)
            {
                exclamationMark.SetActive(true);
            }
            else
            {
                exclamationMark.SetActive(false);
            }
        }
        else
        {
            questionMark.SetActive(false);
            exclamationMark.SetActive(false);
        }
  
    }
    private void LateUpdate()
    {
        questionMark.transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
        exclamationMark.transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
    }
}
