using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolAutoReturn : MonoBehaviour
{
    [SerializeField] private ObjectPoolSO ObjectPoolSO;
    [SerializeField] private float returnDelay = 10f;
    private float timeElapsed;

    private void OnEnable()
    {
        timeElapsed = 0;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= returnDelay)
        {
            ObjectPoolSO.ReturnObject(gameObject);
        }
    }

}
