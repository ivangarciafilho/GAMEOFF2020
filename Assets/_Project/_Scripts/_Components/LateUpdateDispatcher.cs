using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LateUpdateDispatcher : Dispatcher
{
	public override void Reset ( )
	{
		base.Reset ( );
		enabled = true;
	}

	private void LateUpdate ( )
	{
		Dispatch ( );
		if ( notAllowedToDispatch ) enabled = false;
	}
}
