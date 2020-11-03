using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{

    public float delay;
    public UnityEvent triggeredEvents;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(FireDelayedEvents());
    }

    private IEnumerator FireDelayedEvents()
    {
        yield return new WaitForSeconds(delay);
        triggeredEvents.Invoke();
    }
}
