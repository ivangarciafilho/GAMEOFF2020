using System.Collections;
using System.Collections.Generic;

using Bloodstone;

using UltEvents;

using UnityEngine;

public class CycleOfEvents : MonoBehaviour
{
	public UltEvent [ ] sequence;
	[SerializeField, ReadOnly] private int sequenceSize;
	public int currentSequenceIndex;

	public void CycleThrough ( )
	{
		currentSequenceIndex = 
			currentSequenceIndex >= sequenceSize? 
			0 : currentSequenceIndex;

		if ( sequence [ currentSequenceIndex ].HasCalls )
			sequence [ currentSequenceIndex ].Invoke();

		currentSequenceIndex++;
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( sequence == null )
			sequence = new UltEvent [ 0 ];

		sequenceSize = sequence.Length;
	}
#endif
}
