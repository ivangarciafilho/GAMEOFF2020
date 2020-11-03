using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableDipatcher : Dispatcher
{
	private void OnEnable ( )
	{
		Dispatch ( );
	}
}
