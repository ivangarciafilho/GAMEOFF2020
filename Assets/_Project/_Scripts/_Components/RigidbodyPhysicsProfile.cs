using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bloodstone;

public class RigidbodyPhysicsProfile : MonoBehaviour
{
	[SerializeField] private Rigidbody itsRigidbody = null;
	[SerializeField] private RigidbodyPhysicsParameters physicsParametersProfile = null;

	[SerializeField, ReadOnly] private int defaultSolverIterations = 0;
	[SerializeField, ReadOnly] private int defaultSolverVelocityIterations = 0;
	[SerializeField, ReadOnly] private float defaultSleepThreshold = 0f;
	[SerializeField, ReadOnly] private float defaultMaxAngularVelocity = 0f;
	[SerializeField, ReadOnly] private float defaultMaxDepenetrationVelocity = 0f;
	public void ApplyProfile ( )
	{
		if ( physicsParametersProfile == null ) return;

		itsRigidbody.solverIterations = physicsParametersProfile.solverIterations;
		itsRigidbody.solverVelocityIterations = physicsParametersProfile.solverVelocityIterations;
		itsRigidbody.sleepThreshold = physicsParametersProfile.sleepThreshold;
		itsRigidbody.maxAngularVelocity = physicsParametersProfile.maxAngularVelocity;
		itsRigidbody.maxDepenetrationVelocity = physicsParametersProfile.maxDepenetrationVelocity;
	}

	public void RestoreDefaults ( )
	{
		itsRigidbody.solverIterations = defaultSolverIterations;
		itsRigidbody.solverVelocityIterations = defaultSolverVelocityIterations;
		itsRigidbody.sleepThreshold = defaultSleepThreshold;
		itsRigidbody.maxAngularVelocity = defaultMaxAngularVelocity;
		itsRigidbody.maxDepenetrationVelocity = defaultMaxDepenetrationVelocity;
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		itsRigidbody = GetComponent<Rigidbody> ( );

		defaultSolverIterations = itsRigidbody.solverIterations;
		defaultSolverVelocityIterations = itsRigidbody.solverVelocityIterations;
		defaultSleepThreshold = itsRigidbody.sleepThreshold;
		defaultMaxAngularVelocity = itsRigidbody.maxAngularVelocity;
		defaultMaxDepenetrationVelocity = itsRigidbody.maxDepenetrationVelocity;
	}
#endif
}
