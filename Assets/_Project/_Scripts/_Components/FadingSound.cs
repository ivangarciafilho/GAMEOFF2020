using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using DG.Tweening;

using UnityEngine;

public class FadingSound : AudioComponent
{
	public void FadeIn ( )
	{
		StopAllCoroutines ( );
		StartCoroutine ( Fade(1f));
	}

	public void FadeOut ( )
	{
		StopAllCoroutines ( );
		StartCoroutine ( Fade ( 0f ) );
	}


	private IEnumerator Fade ( float  volume)
	{
		while ( itsAudioSource.volume !=  volume )
		{
			var smoothing = Time.smoothDeltaTime * 0.111f;

			itsAudioSource.volume = Mathf.
				Lerp ( itsAudioSource.volume , volume, smoothing);

			itsAudioSource.volume = Mathf.
				MoveTowards ( itsAudioSource.volume, volume, smoothing );

			yield return null;
		}
	}
}
