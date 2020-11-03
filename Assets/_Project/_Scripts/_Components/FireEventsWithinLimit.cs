using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireEventsWithinLimit : MonoBehaviour
{
	public float interval;
	public float availabilitySchedule;
	public UnityEvent triggeredEvents;

	public void FireEvents ( )
	{
		var time = Time.time;

		if ( time < availabilitySchedule ) return;
		availabilitySchedule = time + interval;

		triggeredEvents.Invoke();
	}
}
