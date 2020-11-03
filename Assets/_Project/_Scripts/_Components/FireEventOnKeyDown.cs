using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireEventOnKeyDown : MonoBehaviour
{
	public KeyCode triggeringKey = KeyCode.Space;
	public UnityEvent triggeredEvents;

	private void Update ( )
	{
		if ( Input.GetKeyDown(triggeringKey) )
			triggeredEvents.Invoke ( );
	}
}
