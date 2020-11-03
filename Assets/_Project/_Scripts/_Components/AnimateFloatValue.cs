using System.Collections;
using UnityEngine;

public class AnimateFloatValue : MonoBehaviour
{
	public bool useSmoothing = false;
	
	[Space (16)]

	public AnimationCurve animationValue;
	public float animationLength = 2f;
	public float currentFloatValue;
	public float curveMultiplier = 1f;

	[Space(8)]

	public FloatEvent onStartOfAnimation;
	public FloatEvent passProgress;
	public FloatEvent passValue;
	public FloatEvent onEndOfAnimation;

	private float ellapsedTime;
	private float ellapsedTime_Normalized;

	public void Animate ( )
	{
		if ( isActiveAndEnabled && gameObject.activeInHierarchy)
		{
			ellapsedTime = 0f;
			ellapsedTime_Normalized = 0f;

			StopAllCoroutines ( );
			StartCoroutine ( AnimateValue());
		}
	}

	private IEnumerator AnimateValue ( )
	{
		onStartOfAnimation.
			Invoke ( animationValue.
			Evaluate ( 0 ) 
			* curveMultiplier);

		while ( ellapsedTime_Normalized < 1 )
		{
			ellapsedTime += useSmoothing ? Time.smoothDeltaTime : Time.deltaTime;

			ellapsedTime_Normalized = Mathf.
				Clamp01(ellapsedTime /animationLength);

			passProgress.
				Invoke ( ellapsedTime_Normalized );

			currentFloatValue = (animationValue.
				Evaluate ( ellapsedTime_Normalized )) 
				* curveMultiplier;

			passValue.
				Invoke ( currentFloatValue );

			yield return null;
		}

		onEndOfAnimation.
			Invoke ( animationValue.
			Evaluate ( 1 ) 
			* curveMultiplier);
	}
}
