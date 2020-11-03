using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuouslyRotate : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float startingDelay = 2f;
    private float startingTime;


    private void OnEnable()
    {
        startingTime = Time.time + startingDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < startingTime) return;
        transform.Rotate(0f,speed * Time.smoothDeltaTime, 0f);
    }
}
