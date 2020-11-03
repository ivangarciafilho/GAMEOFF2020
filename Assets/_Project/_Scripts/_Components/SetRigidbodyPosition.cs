using UnityEngine;
using Bloodstone;


public class SetRigidbodyPosition : MonoBehaviour, IConstantReceiver
{
	public Rigidbody itsRigidbody;
	public Transform follow;

	[ReadOnly, SerializeField] private Vector3 _targetPosition;
	public Vector3 targetPosition => _targetPosition;

	public void SetPosition ( Vector3 newPosition )
		=> _targetPosition = newPosition;

	public void AddDirection ( Vector3 newDirection )
	{
		_targetPosition.x += newDirection.x;
		_targetPosition.y += newDirection.y;
		_targetPosition.z += newDirection.z;
	}

	public void OnSignal ( Timeframe timeframe, float timeScale, float time, float delta )
		=> MoveRigidbody();

	public void MoveRigidbody ( )
	{
		if ( follow ) _targetPosition = follow.position;
		itsRigidbody.MovePosition ( _targetPosition );
	}

	public void Reset ( )
	{
		_targetPosition = transform.position;

#if UNITY_EDITOR
		if ( itsRigidbody == null ) return; // yeah, Unity runs any method named "Reset" once the component  is added
#endif

		itsRigidbody.Sleep ( );
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		itsRigidbody = GetComponent<Rigidbody> ( );
	}
#endif
}
