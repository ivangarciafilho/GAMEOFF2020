using System.Collections;
using System.Collections.Generic;
using Bloodstone;
using UnityEngine;

[CreateAssetMenu ( fileName = "RigidbodyParameters", menuName = "Parameters/RigidbodyParameters", order = 1 )]
public class RigidbodyParameters : ScriptableObject
{
	public float mass;
	public float drag;
	public float angularDrag;
	public bool gravityUsage;
	public bool kinematicState;
	public RigidbodyInterpolation interpolation;
	public CollisionDetectionMode collisionDetectionMode;
	[EnumFlag] public RigidbodyConstraints rigidbodyConstrains;

	public int solverIterations;
	public int solverVelocityIterations;
	public float sleepThreshold;
	public float maxAngularVelocity;
	public float maxDepenetrationVelocity;
	public bool collisionDetection;
	public Vector3 centerOfMass;
}

public class RigidbodyParametersProfile : MonoBehaviour
{
	[SerializeField] private Rigidbody itsRigidbody = null;
	[SerializeField] private RigidbodyParameters parametersProfile = null;

	[SerializeField, ReadOnly] 
	private float defaultMass = 0f;

	[SerializeField, ReadOnly] 
	private float defaultDrag = 0f;

	[SerializeField, ReadOnly] 
	private float defaultAngularDrag = 0f;

	[SerializeField, ReadOnly] 
	private bool defaultGravityUsage = true;

	[SerializeField, ReadOnly] 
	private bool defaultKinematicState = true;

	[SerializeField, ReadOnly] 
	private RigidbodyInterpolation defaultInterpolation = RigidbodyInterpolation.None;
	
	[SerializeField, ReadOnly] 
	private CollisionDetectionMode defaultCollisionDetectionMode = CollisionDetectionMode.Discrete;

	[SerializeField, ReadOnly, EnumFlag] 
	private RigidbodyConstraints defaultRigidbodyConstrains = RigidbodyConstraints.None;

	[SerializeField, ReadOnly] private int defaultSolverIterations = 0;
	[SerializeField, ReadOnly] private int defaultSolverVelocityIterations = 0;
	[SerializeField, ReadOnly] private float defaultSleepThreshold = 0;
	[SerializeField, ReadOnly] private float defaultMaxAngularVelocity = 0;
	[SerializeField, ReadOnly] private float defaultMaxDepenetrationVelocity = 0;
	[SerializeField, ReadOnly] private bool defaultCollisionDetection = true;
	[SerializeField, ReadOnly] private Vector3 defaultCenterOfMass = new Vector3(0,0,0);
	public void ApplyProfile ( )
	{
		if ( parametersProfile == null ) return;

		itsRigidbody.mass = parametersProfile.mass;
		itsRigidbody.drag = parametersProfile.drag;
		itsRigidbody.angularDrag = parametersProfile.angularDrag;
		itsRigidbody.useGravity = parametersProfile.gravityUsage;
		itsRigidbody.isKinematic = parametersProfile.kinematicState;
		itsRigidbody.interpolation = parametersProfile.interpolation;
		itsRigidbody.collisionDetectionMode = parametersProfile.collisionDetectionMode;
		itsRigidbody.constraints = parametersProfile.rigidbodyConstrains;

		itsRigidbody.solverIterations = parametersProfile.solverIterations;
		itsRigidbody.solverVelocityIterations = parametersProfile.solverVelocityIterations;
		itsRigidbody.sleepThreshold = parametersProfile.sleepThreshold;
		itsRigidbody.maxAngularVelocity = parametersProfile.maxAngularVelocity;
		itsRigidbody.maxDepenetrationVelocity = parametersProfile.maxDepenetrationVelocity;
		itsRigidbody.detectCollisions = parametersProfile.collisionDetection;
		itsRigidbody.centerOfMass = parametersProfile.centerOfMass;
	}

	public void RestoreDefaults ( )
	{
		itsRigidbody.mass = defaultMass;
		itsRigidbody.drag = defaultDrag;
		itsRigidbody.angularDrag = defaultAngularDrag;
		itsRigidbody.useGravity = defaultGravityUsage;
		itsRigidbody.isKinematic = defaultKinematicState;
		itsRigidbody.interpolation = defaultInterpolation;
		itsRigidbody.collisionDetectionMode = defaultCollisionDetectionMode;
		itsRigidbody.constraints = defaultRigidbodyConstrains;

		itsRigidbody.solverIterations = defaultSolverIterations;
		itsRigidbody.solverVelocityIterations = defaultSolverVelocityIterations;
		itsRigidbody.sleepThreshold = defaultSleepThreshold;
		itsRigidbody.maxAngularVelocity = defaultMaxAngularVelocity;
		itsRigidbody.maxDepenetrationVelocity = defaultMaxDepenetrationVelocity;
		itsRigidbody.detectCollisions = defaultCollisionDetection;
		itsRigidbody.centerOfMass = defaultCenterOfMass;
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		itsRigidbody = GetComponent<Rigidbody> ( );

		defaultMass = itsRigidbody.mass;
		defaultDrag = itsRigidbody.drag;
		defaultAngularDrag = itsRigidbody.angularDrag;
		defaultGravityUsage = itsRigidbody.useGravity;
		defaultKinematicState = itsRigidbody.isKinematic;
		defaultCollisionDetectionMode = itsRigidbody.collisionDetectionMode;
		defaultRigidbodyConstrains = itsRigidbody.constraints;

		defaultSolverIterations = itsRigidbody.solverIterations;
		defaultSolverVelocityIterations = itsRigidbody.solverVelocityIterations;
		defaultSleepThreshold = itsRigidbody.sleepThreshold;
		defaultMaxAngularVelocity = itsRigidbody.maxAngularVelocity;
		defaultMaxDepenetrationVelocity = itsRigidbody.maxDepenetrationVelocity;
		defaultInterpolation = itsRigidbody.interpolation;
		defaultCollisionDetection = itsRigidbody.detectCollisions;
		defaultCenterOfMass = itsRigidbody.centerOfMass;
	}
#endif
}
