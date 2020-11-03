using System.Collections;
using System.Collections.Generic;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VFX_DamageScreen : MonoBehaviour
{
	public PostProcessVolume pfxVolume;
	public float defaultPFXValue = 0f;
	public float fadeSpeed;
	[CurveRange(0,0,1,1)] public AnimationCurve damagePulseAnimation;
	public float pulseDuration;

	public void Update ( )
	{
		var smoothing = Time.smoothDeltaTime * fadeSpeed;
		pfxVolume.weight = Mathf.Lerp ( pfxVolume.weight, defaultPFXValue , smoothing ) ; 
		pfxVolume.weight = Mathf.MoveTowards( pfxVolume.weight, defaultPFXValue , smoothing ) ; 
	}

	public void FadeInDamageScreen ( )
	{
		StopAllCoroutines ( );
		StartCoroutine (Pulse());
	}


	private IEnumerator Pulse ( )
	{
		var ellapsedTime = 0f;
		var ellapsedTimeNormalized = 0f;

		while ( ellapsedTimeNormalized < 1f )
		{
			ellapsedTime += Time.smoothDeltaTime;
			ellapsedTimeNormalized = Mathf.Clamp01(ellapsedTime / pulseDuration);

			pfxVolume.weight = Mathf.Clamp01(pfxVolume.weight 
				+ (Mathf.Clamp01(damagePulseAnimation.Evaluate ( ellapsedTimeNormalized ))));

			yield return null;
		}
	}
}
