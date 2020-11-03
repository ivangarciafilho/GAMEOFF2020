using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static partial class Extensions_Physics
{
	private static readonly Vector3 vector3Zero = Vector3.zero;

	/// <summary>
	/// Body's velocity becomes zero.
	/// </summary>
	/// <param name="body"></param>
	public static void ToZeroVelocity(this Rigidbody body)
	{
		body.velocity = vector3Zero;
	}

	/// <summary>
	/// Body's angular velocity becomes zero.
	/// </summary>
	/// <param name="body"></param>
	public static void ToZeroAngularVelocity(this Rigidbody body)
	{
		body.angularVelocity = vector3Zero;
	}


	/// <summary>
	/// Body's Speed becomes zero and may remove the gravity.
	/// </summary>
	/// <param name="body"></param>
	/// <param name="removeGravity"></param>
	public static void Stop(this Rigidbody body, bool removeGravity = true)
	{
		body.velocity = vector3Zero;
		body.angularVelocity = vector3Zero;
		if ( removeGravity ) body.useGravity = !removeGravity;
		body.Sleep();
	}

	public static void Freeze ( this Rigidbody body)
	{
		body.constraints = RigidbodyConstraints.FreezeAll;
	}

	public static void Unfreeze ( this Rigidbody body)
	{
		body.constraints = RigidbodyConstraints.None;
	}

	public static void StopAndFreeze(this Rigidbody body, bool removeGravity = true)
	{
		body.velocity = vector3Zero;
		body.angularVelocity = vector3Zero;
		body.useGravity = !removeGravity;
		body.constraints = RigidbodyConstraints.FreezeAll;
		body.Sleep ( );
	}

	public static void StopAndDisableCollision ( this Rigidbody body, bool removeGravity = true )
	{
		body.velocity = vector3Zero;
		body.angularVelocity = vector3Zero;
		body.useGravity = !removeGravity;
		body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		body.isKinematic = true;
		body.detectCollisions = false;
		body.Sleep ( );
	}

	public static void DisableCollision(this Rigidbody body)
	{
		body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		body.isKinematic = true;
		body.detectCollisions = false;
		body.Sleep();
	}

	public static void EnableCollision(this Rigidbody body)
	{
		body.isKinematic = false;
		body.collisionDetectionMode = CollisionDetectionMode.Discrete;
		body.detectCollisions = true;
		body.WakeUp();
	}

	public static void SwitchCollisionDetection(this Rigidbody body, bool allowCollisions)
	{
		if (allowCollisions)
		{
			body.isKinematic = false;
			body.collisionDetectionMode = CollisionDetectionMode.Discrete;
			body.detectCollisions = true;
			body.WakeUp();
		}
		else
		{
			body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			body.isKinematic = true;
			body.detectCollisions = false;
			body.Sleep();
		}

		body.detectCollisions = allowCollisions;
	}

	public static void SwitchCollisionDetection (this Rigidbody body, bool isKinematic, CollisionDetectionMode newCollisionDetectionMode)
	{
		if (isKinematic)
		{
			body.collisionDetectionMode = newCollisionDetectionMode;
			body.isKinematic = true;
		}
		else
		{
			body.isKinematic = false;
			body.collisionDetectionMode = newCollisionDetectionMode;
		}
	}

	public static void SwitchCollisionDetection (this Rigidbody body, bool isKinematic, bool collisionDetection, CollisionDetectionMode newCollisionDetectionMode)
	{
		body.SwitchCollisionDetection (body, isKinematic, newCollisionDetectionMode);
		body.detectCollisions = collisionDetection;
	}

	public static void SwitchCollisionDetection (this Rigidbody body, bool isKinematic, CollisionDetectionMode newCollisionDetectionMode, bool stopRigidbody)
	{
		if (stopRigidbody) body.Stop();
		body.SwitchCollisionDetection (body, isKinematic, newCollisionDetectionMode);
	}

	public static void SwitchCollisionDetection (this Rigidbody body, bool isKinematic, bool collisionDetection, CollisionDetectionMode newCollisionDetectionMode, bool stopRigidbody)
	{
		if (stopRigidbody) body.Stop();
		body.SwitchCollisionDetection (body, isKinematic, newCollisionDetectionMode);
		body.detectCollisions = collisionDetection;
	}


	public static void IgnoreCollision(this Collider ownCollider, Collider otherCollider, bool ignoreCollision = true)
	{
		Physics.IgnoreCollision(ownCollider,otherCollider,ignoreCollision);
	}

	public static void IgnoreCollision(this Collider ownCollider, Collider[] colliderGroup, bool ignoreCollision = true)
	{
		var amountOfColliders = colliderGroup.Length;

		for (int i = 0; i < amountOfColliders; i++)
			Physics.IgnoreCollision(ownCollider, colliderGroup[i], ignoreCollision);
	}

	public static void IgnoreCollision(this Collider[] colliderGroup, Collider targetCollider, bool ignoreCollision = true)
	{
		var amountOfColliders = colliderGroup.Length;

		for (int i = 0; i < amountOfColliders; i++)
			Physics.IgnoreCollision(colliderGroup[i], targetCollider, ignoreCollision);
	}

	public static void IgnoreCollision(this Collider[] colliderGroupA, Collider[] colliderGroupB, bool ignoreCollision = true)
	{
		var amountOfCollidersOnGroupA = colliderGroupA.Length;
		var amountOfCollidersOnGroupB = colliderGroupB.Length;

		for (int a = 0; a < amountOfCollidersOnGroupA; a++)
		{
			for (int b = 0; b < amountOfCollidersOnGroupB; b++)
			{
				Physics.IgnoreCollision(colliderGroupA[a], colliderGroupB[b], ignoreCollision);
			}
		}
	}

	public static bool Intersects ( this Collider[] colliders, Collider other, float precisionThrehold = 0f)
	{
		var amountOfColliders = colliders.Length;
		var intersected = false;

		for ( int i = 0; i < amountOfColliders; i++ )
		{
			var intersections = colliders [ i ].ClosestPointsBetweenColliders( other );

			if ( intersections.distanceBewteenPoints < ((precisionThrehold == 0)? Physics.defaultContactOffset : precisionThrehold) )
			{
				intersected = true;
				break;
			}
		}

		return intersected;
	}

	public static bool Instersects ( this Collider collider, Collider[] otherColliders )
	{
		return otherColliders.Intersects(collider);
	}

	public static bool Instersects ( this Collider[] thisCollider, Collider [ ] otherColliders )
	{
		var amountOfColliders = thisCollider.Length;
		var intersected = false;

		for ( int i = 0; i < amountOfColliders; i++ )
		{
			if ( thisCollider [ i ].Instersects ( otherColliders ) )
			{
				intersected = true;
				break;
			}
		}

		return intersected;
	}

	public static bool Contains ( this Collider[] thisColliders, Vector3 point)
	{
		var amountOfColliders = thisColliders.Length;
		var contains = false;

		for ( int i = 0; i < amountOfColliders; i++ )
		{
			if ( thisColliders [ i ].bounds.Contains ( point ) )
			{
				contains = true;
				break;
			}
		}

		return contains;
	}

	public static bool Contains ( this Collider [ ] thisColliders, Transform transform )
	{
		return thisColliders.Contains(transform.position);
	}

	public static bool WithinBounds ( this Vector3 point, Collider collider )
	{
		return collider.bounds.Contains ( point );
	}

	public static bool WithinBounds ( this Transform thisTransform, Collider collider )
	{
		return thisTransform.position.WithinBounds(collider);
	}

	public static (Vector3 onThisCollider, Vector3 onOtherCollider, float distanceBewteenPoints ) ClosestPointsBetweenColliders(this Collider thisCollider, Collider otherCollider)
	{
		var closestPointOnThisCollider = thisCollider.bounds.ClosestPoint(otherCollider.transform.position);
		var closestPointOnOtherCollider = otherCollider.bounds.ClosestPoint(thisCollider.transform.position);
		
		closestPointOnThisCollider = thisCollider.bounds.ClosestPoint(closestPointOnOtherCollider);
		closestPointOnOtherCollider = otherCollider.bounds.ClosestPoint(closestPointOnThisCollider);

		closestPointOnThisCollider = thisCollider.bounds.ClosestPoint(closestPointOnOtherCollider);
		closestPointOnOtherCollider = otherCollider.bounds.ClosestPoint(closestPointOnThisCollider);

		return (closestPointOnThisCollider, closestPointOnOtherCollider, Vector3.Distance( closestPointOnThisCollider, closestPointOnOtherCollider ));
	}

	public static (float closestDistance, Vector3 closestPointOnThisCollider, Vector3 closestPointOnOtherCollider) 
		ClosestDistanceBetweenColliders(this Collider thisCollider, Collider otherCollider)
	{
		var closestPoints = thisCollider.ClosestPointsBetweenColliders(otherCollider);
		return (Vector3.Distance(closestPoints.onThisCollider, closestPoints.onOtherCollider),
			closestPoints.onThisCollider, closestPoints.onOtherCollider);
	}

	public static Vector3 DirectionTo(this Vector3 point, Vector3 otherPoint, bool normalized = true)
	{
		return normalized? (otherPoint - point).normalized : (otherPoint - point);
	}

	public static Quaternion RotationTo(this Vector3 point, Vector3 otherPoint, Vector3 upVector)
	{
		return Quaternion.LookRotation(point.DirectionTo(otherPoint), upVector);
	}

	public static void Enabled(this IList<Collider> items, bool enabled)
	{
		var itemsCount = items.Count;

		for (int i = 0; i < itemsCount; i++)
			items[i].enabled = enabled;
	}

	public static void EnableAll(this IList<Collider> items)
	{
		var itemsCount = items.Count;

		for (int i = 0; i < itemsCount; i++)
			items[i].enabled = true;
	}

	public static void Enable(this Collider collider)
	{
		collider.enabled = true;
	}

	public static void Disable(this Collider collider)
	{
		collider.enabled = false;
	}

	public struct SurfaceHit
	{
		public Collider collider;
		public Vector3 point;
		public float distance;

		public SurfaceHit ( Collider collider, Vector3 point, float distance )
		{
			this.collider = collider;
			this.point = point;
			this.distance = distance;
		}
	}

	public struct SurfaceHitPair
	{
		public SurfaceHit surfaceHitA;
		public SurfaceHit surfaceHitB;

		public SurfaceHitPair ( SurfaceHit surfaceHitA, SurfaceHit surfaceHitB )
		{
			this.surfaceHitA = surfaceHitA;
			this.surfaceHitB = surfaceHitB;
		}
	}

	public static 
		(
		float distance, 
		Vector3 pointOnThisObject, 
		Vector3 pointOnOtherObject, 
		Collider thisClosestCollider,
		Collider otherClosestCollider,
		SurfaceHit thisObject, 
		SurfaceHit otherObject, 
		SurfaceHitPair pairData
		) ClosestPointsBetweenObjects ( this Collider[] objectA,  Collider[] objectB)
	{
		var amountOfCollidersOnObjectA = objectA.Length;
		var amountOfCollidersOnObjectB = objectB.Length;
		Collider colliderFromObjectABeingProccessed;
		Collider colliderFromObjectBBeingProccessed;

		var proccessedPointsFromObjectA = new SurfaceHit[amountOfCollidersOnObjectA];
		var closestPairedPoints = new SurfaceHitPair[amountOfCollidersOnObjectA];

		var comparedPointsFromObjectA = new SurfaceHit[amountOfCollidersOnObjectB];
		var proccessedPointsFromObjectB = new SurfaceHit[amountOfCollidersOnObjectB];
		var pairedPointsFromObjects = new SurfaceHitPair[amountOfCollidersOnObjectB];

		for ( int a = 0; a < amountOfCollidersOnObjectA; a++ )
		{
			colliderFromObjectABeingProccessed = objectA [ a ];

			for ( int b = 0; b < amountOfCollidersOnObjectB; b++ )
			{
				colliderFromObjectBBeingProccessed = objectB [ b ];

				//Get Points
				var proximityData = colliderFromObjectABeingProccessed.ClosestPointsBetweenColliders ( colliderFromObjectBBeingProccessed );

				comparedPointsFromObjectA [ b ].collider = colliderFromObjectABeingProccessed;
				comparedPointsFromObjectA [ b ].point = proximityData.onThisCollider;

				proccessedPointsFromObjectB [ b ].collider = colliderFromObjectBBeingProccessed ;
				proccessedPointsFromObjectB [ b ].point = proximityData.onOtherCollider;

				//Get Distance
				var distance = Vector3.Distance ( comparedPointsFromObjectA [ b ].point, proccessedPointsFromObjectB [ b ].point );
				comparedPointsFromObjectA [ b ].distance = distance;
				proccessedPointsFromObjectB [ b ].distance = distance;

				//Store proximity Data for later Filtering
				pairedPointsFromObjects [ b ].surfaceHitA = comparedPointsFromObjectA[b];
				pairedPointsFromObjects [ b ].surfaceHitB = proccessedPointsFromObjectB[b];
			}

			//Keep the shortest Pair distance, discard the rest
			var minimumDistance = pairedPointsFromObjects.Min (_item =>  _item.surfaceHitA.distance );
			closestPairedPoints [ a ] = pairedPointsFromObjects.First ( _item => _item.surfaceHitA.distance == minimumDistance );
		}

		//Return only the closest Pair
		var closestPoints = closestPairedPoints.Min ( _Item => _Item.surfaceHitA.distance);
		var closestPair = closestPairedPoints.First (_item => _item.surfaceHitA.distance == closestPoints);

		return (closestPair.surfaceHitA.distance, closestPair.surfaceHitA.point, closestPair.surfaceHitB.point, closestPair.surfaceHitA.collider, closestPair.surfaceHitB.collider, closestPair.surfaceHitA, closestPair.surfaceHitB, closestPair);
	}
}


