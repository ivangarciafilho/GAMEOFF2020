using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public partial class Extensions
{
	public static readonly Vector3 vector3Forward = new Vector3 ( 0f, 0f, 1f );
	public static readonly Vector3 vector3Up = new Vector3 ( 0f, 1f, 0f );

	public static float DotTowards ( this Transform transform, Transform target)
	{
		var transformForward = transform.TransformDirection(vector3Forward);
		var targetDirection = (target.position - transform.position).normalized;

		return Vector3.Dot ( transformForward, targetDirection);
	}

	public static float DotTowards ( this Transform transform, Vector3 point )
	{
		var transformForward = transform.TransformDirection ( vector3Forward );
		var pointDirection = ( point - transform.position ).normalized;

		return Vector3.Dot ( transformForward, pointDirection );
	}

	public static Vector3 DirectionFrom ( this Vector3 thisPoint, Vector3 targetPoint )
	{
		return (thisPoint - targetPoint).normalized;
	}

	public static Vector3 DirectionTo ( this Vector3 thisPoint, Vector3 targetPoint )
	{
		return (targetPoint - thisPoint).normalized;
	}

	public static Quaternion ToLookRotation ( this Vector3 eulerRotation)
		=> Quaternion.LookRotation(eulerRotation.normalized, vector3Up);

	public static (float distance, Vector3 direction, Vector3 diference)  
		To ( this Vector3 origin, Vector3 destination )
	{
		var resultingDifference = destination;
		resultingDifference.x -= origin.x;
		resultingDifference.y -= origin.y;
		resultingDifference.z -= origin.z;

		var resultingDistance  =
				(( resultingDifference.x * resultingDifference.x )
			+	( resultingDifference.y * resultingDifference.y )
			+	( resultingDifference.z * resultingDifference.z )).
			FastInvSqrt();

		var resultingDirection = resultingDifference;
		resultingDirection.x /= resultingDistance;
		resultingDirection.y /= resultingDistance;
		resultingDirection.z /= resultingDistance;

		return (resultingDistance, resultingDirection, resultingDifference);
	}


	public static (float distance, Vector3 direction, Vector3 diference)  
		To ( this Vector3 origin, Transform target)
		=> origin.To(target.position);

	public static (float distance, Vector3 direction, Vector3 diference) 
		To ( this Transform thisTransform, Transform targetTransform )
		=> thisTransform.position.To(targetTransform.position);

	public static (float distance, Vector3 direction, Vector3 diference)  
		To ( this Transform thisTransform, Vector3 point )
		=> thisTransform.position.To(point);
}