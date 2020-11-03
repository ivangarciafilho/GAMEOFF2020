using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RepeatingEvent : MonoBehaviour
{
    public Vector2 delay;
    public UnityEvent triggeredEvents;
    public bool keepFiring = true;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(FireDelayedEvents());
    }

    private IEnumerator FireDelayedEvents()
    {
        while (true)
        {
            while (keepFiring == false) yield return null;

            var randomDelay = Random.Range(delay.x,delay.y);
            yield return new WaitForSeconds(randomDelay);
            triggeredEvents.Invoke();
        }
    }
}
