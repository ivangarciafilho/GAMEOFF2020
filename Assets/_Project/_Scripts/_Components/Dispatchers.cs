using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dispatcher : MonoBehaviour
{
	public UnityEvent triggeredEvents;
	[SerializeField] protected bool dispatchOnce = true;
	[SerializeField] protected bool alreadyDispatched;
	public virtual bool notAllowedToDispatch { get { return ( dispatchOnce == true && alreadyDispatched == true ); } }

	public virtual void Reset ( )
	{
		alreadyDispatched = false;
	}

	protected virtual void Dispatch ( )
	{
		if ( notAllowedToDispatch ) return;

		triggeredEvents.Invoke ( );
		alreadyDispatched = true;
	}
}
