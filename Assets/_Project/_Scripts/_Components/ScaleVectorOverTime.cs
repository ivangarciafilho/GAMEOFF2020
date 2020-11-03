using System;
using Bloodstone;
using UnityEngine;
using UltEvents;
using UnityEngine.Rendering;

[Serializable] public class Vector3ValueEvent : UltEvent<Vector3> { }

public class ScaleVectorOverTime : MonoBehaviour
{
	public AnimationCurve scaleX;
	public AnimationCurve scaleY;
	public AnimationCurve scaleZ;

	[ReadOnly] public float generatedScaleMultiplier = 90f;
	public Vector2 scaleMultiplierRange = new Vector2(0f , 1f);

	public void SetScaleMultiplier ( float value )
		=> scaleMultiplierRange = Vector2.one * value;

	public void SetScaleMultiplier ( Vector2 value )
		=> scaleMultiplierRange = value;

	[ReadOnly] private Vector3 _currentVectorScale;

	public Vector3 currentVectorScale => _currentVectorScale;

	[ReadOnly] public float generatedScalingDelay;
	public Vector2 scalingDelayRange;

	public void SetScalingDelay ( float value )
		=> scalingDelayRange = Vector2.one * value;

	public void SetScalingDelay ( Vector2 value )
		=> scalingDelayRange = value;

	[ReadOnly] private float ellapsedTime;
	[ReadOnly] private float ellapsedTimeNormalized;

	
	public Vector3ValueEvent onStartOfScaling;
	public Vector3ValueEvent whileScaling;
	public Vector3ValueEvent onEndOfScaling;

	public void AnimateScale (float timescale, float time,  float fixedDeltatime,  float  smoothDeltatime)
	{
		if ( ellapsedTimeNormalized == 0 )
		{
			if ( onStartOfScaling.HasCalls ) 
				onStartOfScaling.
				Invoke ( _currentVectorScale );

			GenerateAnimationValues ( );
		}

		if ( ellapsedTimeNormalized < 1 )
		{
			ellapsedTime += fixedDeltatime;

			ellapsedTimeNormalized = (ellapsedTimeNormalized < 1)
				? ellapsedTime / generatedScalingDelay : 1;

			_currentVectorScale.x = scaleX.
				Evaluate ( ellapsedTimeNormalized )  
				* generatedScaleMultiplier;

			_currentVectorScale.y = scaleY.
				Evaluate ( ellapsedTimeNormalized ) 
				* generatedScaleMultiplier;

			_currentVectorScale.z = scaleZ.
				Evaluate ( ellapsedTimeNormalized ) 
				* generatedScaleMultiplier;

			if ( whileScaling.HasCalls ) 
				whileScaling.
					Invoke ( _currentVectorScale );
		}

		if ( ellapsedTimeNormalized >= 1 
			&& onEndOfScaling.HasCalls) 
			onEndOfScaling.
				Invoke ( _currentVectorScale );
	}

	public void Reset ( )
	{
		ellapsedTime = 0;
		ellapsedTimeNormalized = 0;
	}

	public void GenerateAnimationValues( )
	{
		generatedScaleMultiplier = 
				scaleMultiplierRange.
				RandomInRange ( );

			generatedScalingDelay = 
				scalingDelayRange.
				RandomInRange ( );
	}
}
