using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Bloodstone;

using UnityEngine;

public class FadeInAndOutRandomly : AudioComponent
{
	public AnimationCurve volumeCurve_0;
	public float chanceToSwithTo_1;

	public AnimationCurve volumeCurve_1;
	public float chanceToSwithTo_0;

	[ReadOnly, SerializeField] private float cycleLength;
	[ReadOnly, SerializeField] private float nextCycle;

	public int currentVolumeIndex = 0;

	private void OnEnable ( )
	{
		StopAllCoroutines ( );
		StartCoroutine(HandleProceduralFading ( ));
	}

	private IEnumerator HandleProceduralFading( )
	{
		cycleLength = itsAudioSource.clip.length;
		nextCycle = 0f;

		while ( true )
		{
			if ( Time.time >= nextCycle )
			{
				nextCycle = Time.time + cycleLength;
				if ( currentVolumeIndex == 0 )
				{
					if ( Random.value < chanceToSwithTo_1 )
					{
						currentVolumeIndex = 1;
						StartCoroutine( Fade ( volumeCurve_1 ));
					}
				}
				else
				{
					if ( Random.value < chanceToSwithTo_0 )
					{
						currentVolumeIndex = 0;
						StartCoroutine( Fade ( volumeCurve_0 ));
					}
				}
			}

			yield return null;
		}
	}

	private IEnumerator Fade ( AnimationCurve animationCurve)
	{
		var ellapsedTime = 0f;
		var ellapsedTime_Normalized = 0f;
		var fadeDuration = animationCurve.keys.Last ( ).time;

		itsAudioSource.volume = animationCurve.Evaluate ( 0 );

		while ( ellapsedTime_Normalized < 1f )
		{
			ellapsedTime += Time.deltaTime;
			ellapsedTime_Normalized = Mathf.Clamp01 ( ellapsedTime / fadeDuration);

			itsAudioSource.volume = animationCurve.
				Evaluate ( ellapsedTime_Normalized );

			yield return null;
		}

		itsAudioSource.volume = animationCurve.Evaluate ( fadeDuration );
	}
}
