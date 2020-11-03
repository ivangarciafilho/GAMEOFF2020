using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMonitor : MonoBehaviour
{
	public Health verifiedHealth;
	public AnimationCurve valueRemap;
	public FloatEvent passEvaluatedValue;
	
	public float currentValue;
	public float currentValueRemapped;

	private IEnumerator Start ( )
	{
		yield return null;

		currentValue = verifiedHealth.currentHealthNormalized;
		currentValueRemapped = 0f;
		passEvaluatedValue.Invoke ( 0 );
	}


	private void Update ( )
	{
		var smoothing = Time.smoothDeltaTime * 0.333f;

		currentValue = Mathf.
			Lerp (currentValue , verifiedHealth.currentHealthNormalized, smoothing);

		currentValue = Mathf.
			MoveTowards (currentValue , verifiedHealth.currentHealthNormalized, smoothing);

		currentValueRemapped = valueRemap.
			Evaluate ( currentValue );

		passEvaluatedValue.Invoke (currentValueRemapped );
	}
}
