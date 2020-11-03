using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Bloodstone;

using UnityEngine;

public class CycleOfEventGroups : MonoBehaviour
{
	public GroupOfEvents[] sequence;
	[SerializeField, ReadOnly] private int sequenceSize;
	[SerializeField, ReadOnly] private int currentSequenceIndex;

	public void CycleThrough ( )
	{
		currentSequenceIndex =
			currentSequenceIndex >= sequenceSize ?
			0 : currentSequenceIndex;

		sequence [ currentSequenceIndex ].FireGroup ( );

		currentSequenceIndex++;
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		sequence = sequence.
			Where ( _item => _item != null).
			ToArray();

		sequence = sequence.
			Where ( _item => _item.events.HasCalls ).
			ToArray ( );

		sequenceSize = sequence.Length;
	}
#endif
}
