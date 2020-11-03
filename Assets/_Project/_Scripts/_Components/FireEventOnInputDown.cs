using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

public class FireEventOnInputDown : MonoBehaviour
{
    public KeyCode key;
    public UltEvent triggeredEvents;

    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            triggeredEvents.Invoke();
        }
    }
}
