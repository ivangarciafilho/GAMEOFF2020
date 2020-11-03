using UnityEngine;
using NaughtyAttributes;

public class ScaleFloatOverTime : MonoBehaviour, IConstantReceiver
{
	 public AnimationCurve scalingCurve;

	[ReadOnly,SerializeField] private float _generatedScaleMultiplier;
	public float generatedScaleMultiplier => _generatedScaleMultiplier;

	[SerializeField]  private Vector2 _scaleMultiplierRange = new Vector2(0.5f,1.5f);
	public Vector2 scaleMultiplierRange =>_scaleMultiplierRange;

	public Vector2 SetScaleMultiplier ( float value )
		=> _scaleMultiplierRange = Vector2.one * value;

	public Vector2 SetScaleMultiplier ( Vector2 value )
		=> _scaleMultiplierRange = value;

	[ReadOnly,SerializeField] private float _generatedDelay = 0f;
	public float generatedDelay =>_generatedDelay;

	[SerializeField] private Vector2 _scalingDelayRange = new Vector2(0.5f, 1.5f);
	public Vector2 scalingDelayRange => _scalingDelayRange;

	public Vector2 SetScalingDelay ( float value )
		=> _scalingDelayRange = Vector2.one *  value;

	public Vector2 SetScalingDelay ( Vector2 value )
		=> _scalingDelayRange = value;

	[ReadOnly,SerializeField] private float _currentScale;

	public float currentScale => _currentScale;

	[ReadOnly, SerializeField] private float ellapsedTime;
	[ReadOnly, SerializeField] private float ellapsedTimeNormalized;


	public FloatValueEvent onStartOfScaling;
	public FloatValueEvent whileScaling;
	public FloatValueEvent onEndOfScaling;

	public void OnSignal ( Timeframe timeframe, float timeScale, float time, float delta )
		=> AnimateScale (timeframe, timeScale, time, delta  );

	public void AnimateScale ( Timeframe timeframe, float timeScale, float time,  float fixedDeltatime )
		=> AnimateScale (fixedDeltatime);

	public void AnimateScale (float fixedDeltatime)
	{
		if ( ellapsedTimeNormalized == 0 )
		{
			if ( onStartOfScaling.HasCalls ) 
				onStartOfScaling.
				Invoke ( _currentScale  );

			GenerateAnimationValues ( );
		}

		if ( ellapsedTimeNormalized < 1 )
		{
			ellapsedTime += fixedDeltatime;

			ellapsedTimeNormalized = (ellapsedTimeNormalized  < 1) 
				? ellapsedTime / generatedDelay : 1;

			_currentScale = scalingCurve.
				Evaluate ( ellapsedTimeNormalized )  
				* generatedScaleMultiplier;

			if ( whileScaling.HasCalls ) 
				whileScaling.
					Invoke ( _currentScale );
		}

		if ( ellapsedTimeNormalized >= 1 
			&& onEndOfScaling.HasCalls) 
			onEndOfScaling.
				Invoke ( _currentScale  );
	}

	public void Reset ( )
	{
		ellapsedTime = 0;
		ellapsedTimeNormalized = 0;

		GenerateAnimationValues ( );

		if ( scalingCurve != null )
		_currentScale = scalingCurve.Evaluate(0) 
				* generatedScaleMultiplier;
		
	}

	public void GenerateAnimationValues( )
	{
		_generatedScaleMultiplier = 
			_scaleMultiplierRange.
			RandomInRange ( );

		_generatedDelay = 
			_scalingDelayRange.
			RandomInRange ( );
	}
}
