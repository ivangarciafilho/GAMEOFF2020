using System.Collections;
using System.Collections.Generic;

using UltEvents;

using UnityEngine;

public class GroupOfEvents : MonoBehaviour
{
	public UltEvent events;
	public void FireGroup ( )
	{
		if ( events.HasCalls )
			events.Invoke ( );
	}
}
