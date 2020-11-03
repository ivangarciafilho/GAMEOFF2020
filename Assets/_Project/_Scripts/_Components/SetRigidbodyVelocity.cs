using Bloodstone;
using UnityEngine;

public class SetRigidbodyVelocity : MonoBehaviour, IConstantReceiver
{
	public Rigidbody itsRigidbody;
	public float velocityConservation = 0;

	[ReadOnly] private Vector3 _currentVelocity;
	public Vector3 currentVelocity => _currentVelocity;

	[ReadOnly] private Vector3 _currentVelocityAngular;
	public Vector3 currentVelocityAngular => _currentVelocityAngular;

	public void SetVelocity ( Vector3 newMotion )
		=> _currentVelocity = newMotion;

	public void OnSignal ( Timeframe timeframe, float timeScale, float time, float delta )
		=> ApplyCurrentVelocity ( );

	public void AddVelocity ( Vector3 newMotion)
	{
		_currentVelocity.x += newMotion.x;
		_currentVelocity.y += newMotion.y;
		_currentVelocity.z += newMotion.z;
	}

	public void ApplyCurrentVelocity ( )
	{
		itsRigidbody.velocity = _currentVelocity;
		itsRigidbody.angularVelocity = _currentVelocityAngular;

		_currentVelocity.x *= velocityConservation;
		_currentVelocity.y *= velocityConservation;
		_currentVelocity.z *= velocityConservation;
	}

	public void _Reset ( )
	{

#if UNITY_EDITOR
		if ( itsRigidbody == null ) return; //Yeah, Unity runs any method named "Reset" once the component  is added
#endif

		var zeroVelocity = Vector3.zero;
		itsRigidbody.velocity = zeroVelocity ;
		itsRigidbody.angularVelocity = zeroVelocity;
		_currentVelocity = zeroVelocity;
		_currentVelocityAngular = zeroVelocity;
		itsRigidbody.Sleep ( );
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		itsRigidbody = GetComponent<Rigidbody> ( );
	}
#endif
}
