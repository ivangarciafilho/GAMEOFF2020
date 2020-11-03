using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

[Serializable] public class FloatEvent : UltEvent<float> { }

public class CaptureInputAxis : MonoBehaviour
{
	public float currentHorizontalAxisImput;
	public float currentVerticalAxisImput;
	public FloatEvent passHorizontalAxisValue;
	public FloatEvent passsVerticalAxisValue;

   public void Capture ( )
	{
		currentHorizontalAxisImput = Input.GetAxis ( "Horizontal" );
		if ( passHorizontalAxisValue.HasCalls )
			passHorizontalAxisValue.Invoke ( currentHorizontalAxisImput );

		currentVerticalAxisImput = Input.GetAxis ( "Vertical" );
		if ( passsVerticalAxisValue.HasCalls )
			passsVerticalAxisValue.Invoke ( currentVerticalAxisImput);
	}
}
