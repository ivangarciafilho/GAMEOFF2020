using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireEventAfterDelay : MonoBehaviour
{
	public float delay;
	public UnityEvent triggeredEvents;
	private float triggeringTime;
	private bool alreadyTriggered;

	private void OnEnable ( )
	{
		triggeringTime = Time.time + delay;
		alreadyTriggered = false;
	}

	private void Update ( )
	{
		if ( Time.time < triggeringTime && alreadyTriggered == false) return;
		Fire ( );
	}


	private void Fire ( )
	{
		triggeredEvents.Invoke ( );
		alreadyTriggered = true;
	}
}
