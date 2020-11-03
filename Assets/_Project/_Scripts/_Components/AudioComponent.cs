using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioComponent : MonoBehaviour
{
	[SerializeField] private AudioSource _itsAudioSource;
	public AudioSource itsAudioSource => _itsAudioSource;

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( _itsAudioSource == null) _itsAudioSource = GetComponentsInChildren<AudioSource> ( ) [ 0 ];
	}
#endif
}
