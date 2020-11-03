using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyHealthUpdate	: MonoBehaviour
{
	public Collider colliderReference;
	public float value;

	public void Apply ( )
	{
		Damageable.UpdateHealth (colliderReference.gameObject.GetInstanceID(),GenerateDamage);
	}

	private float GenerateDamage ( Damageable damageable)
	{
		return Random.Range ( 0.666f, 0.999f ) * - Mathf.Abs(value);
	}
}
