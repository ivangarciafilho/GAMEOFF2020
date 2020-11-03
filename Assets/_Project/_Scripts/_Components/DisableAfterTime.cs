using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public float disableAfterSecs = 2.0f;
    float elapsedTime;

    private void OnEnable()
    {
        elapsedTime = Time.time + disableAfterSecs;
    }

    private void OnDisable()
    {
        elapsedTime = Time.time + disableAfterSecs;
    }

    private void Update()
    {
        if (Time.time > elapsedTime)
            gameObject.SetActive(false);
    }
}
