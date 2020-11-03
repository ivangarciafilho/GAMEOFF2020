using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class FilterValue : MonoBehaviour
{
	public float threshold;

	public UltEvent higher;
	public UltEvent equals;
	public UltEvent lower;

	public void Evaluate ( float value)
	{
		if ( (value > threshold)  && higher.HasCalls )
		{
			higher.Invoke ( );
			return;
		}

		if ( (value == threshold) && equals.HasCalls )
		{
			equals.Invoke ( );
			return;
		}

		if ( (value < threshold) && lower.HasCalls )
		{
			lower.Invoke ( );
			return;
		}
	}
}
