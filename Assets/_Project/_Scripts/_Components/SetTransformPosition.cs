using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransformPosition : MonoBehaviour
{
	public Vector3 position;
	public Vector3 direction;
	public float directionConservation;
	

	public void ApplyPosition ( )
		=>transform.position = position;

	public void ApplyDirection ( )
	{
		transform.position += direction;
		direction *= directionConservation;
	}
}
