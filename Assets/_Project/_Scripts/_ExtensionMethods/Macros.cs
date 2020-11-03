using UnityEngine;

public static class Macros
{
	public static void RemoveFromPhysics ( Rigidbody body, Collider [ ] colliders )
	{
		body.StopAndFreeze ( );
		colliders.SetEnabled ( );
		body.DisableCollision ( );
	}

	public static void RemoveFromPhysics ( Rigidbody body, Collider collider )
	{
		body.StopAndFreeze ( );
		collider.enabled = false;
		body.DisableCollision ( );
	}

	public static void ReturnToPhysics ( Rigidbody body, Collider [ ] colliders )
	{
		body.constraints = RigidbodyConstraints.None;
		colliders.EnableAll ( );
		body.EnableCollision ( );
	}

	public static void ReturnToPhysics ( Rigidbody body, Collider collider )
	{
		body.constraints = RigidbodyConstraints.None;
		collider.enabled = true;
		body.EnableCollision ( );
		body.WakeUp ( );
	}
}
