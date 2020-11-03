using UnityEngine;

partial class Extensions_Physics
{
	public static void SetPhysicsParams ( this Rigidbody body, RigidbodyPhysicsParameters parameters )
	{
		body.solverIterations = parameters.solverIterations;
		body.solverVelocityIterations = parameters.solverVelocityIterations;
		body.sleepThreshold = parameters.sleepThreshold;
		body.maxAngularVelocity = parameters.maxAngularVelocity;
		body.maxDepenetrationVelocity = parameters.maxDepenetrationVelocity;
	}
}

[CreateAssetMenu ( fileName = "RigidbodyPhysicsParameters", menuName = "Parameters/RigidbodyPhysicsParameters", order = 1 )]
public class RigidbodyPhysicsParameters : ScriptableObject
{
	public int solverIterations;
	public int solverVelocityIterations;
	public float sleepThreshold;
	public float maxAngularVelocity;
	public float maxDepenetrationVelocity;
}
