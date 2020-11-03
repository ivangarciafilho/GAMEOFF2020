using UnityEngine;
using Bloodstone;

public class AddVelocityTowardsClosestEntity : MonoBehaviour
{
	public Rigidbody itsRigidbody;
	public EntitiesWithinRangeStorage itsStoredEntitiesInRange;
	public SetRigidbodyVelocity itsApplyRigidbodyVelocity;

	private Vector3 velocityDirection;

	public float driftingVelocity = 3;
	public void SetDriftingVelocity ( float value )
		=> driftingVelocity = value;

	public float correctionVelocity = 3f;
	public void SetCorrectionVelocity ( float value )
		=> correctionVelocity = value;

	[ReadOnly] private PairSpacialRelationship _closestEntity;
	public PairSpacialRelationship closestEntity => _closestEntity;

	[ReadOnly] private Vector3 _closestEntityDirection;
	public Vector3 closestEntityDirection => _closestEntityDirection;

	[ReadOnly] private Quaternion _currentRotationAdjustment;
	public Quaternion currentRotationAdjustment => _currentRotationAdjustment;

	public void SeekClosest (float timescale, float time, float fixedDeltatime, float  smoothDeltatime)
	{
		var amountOfEntities = itsStoredEntitiesInRange.EntitiesWithinRange.Length;
		_closestEntityDirection = Vector3.zero;

		if ( amountOfEntities > 0 ) 
		{
			_closestEntity = itsStoredEntitiesInRange.EntitiesWithinRange[0];

			_currentRotationAdjustment = Quaternion.
				RotateTowards ( itsRigidbody.rotation,
				Quaternion.LookRotation ( _closestEntity.directionToB ),
				fixedDeltatime * correctionVelocity );

			itsRigidbody.MoveRotation ( _currentRotationAdjustment );
		}

		itsApplyRigidbodyVelocity.AddVelocity( closestEntityDirection * driftingVelocity);
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( itsRigidbody == null )
			itsRigidbody = GetComponent<Rigidbody> ( );

		if ( itsStoredEntitiesInRange  == null )
			itsStoredEntitiesInRange = GetComponent<EntitiesWithinRangeStorage> ( );

		if ( itsApplyRigidbodyVelocity  == null )
			itsApplyRigidbodyVelocity = GetComponent<SetRigidbodyVelocity> ( );
	}
#endif
}
