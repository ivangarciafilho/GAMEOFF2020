using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bloodstone;

public class ApplyRigidbodyRotation : MonoBehaviour
{
	public Rigidbody itsRigidbody;

	[ReadOnly] private Quaternion _currentMotion;
	public Quaternion currentMotion => _currentMotion;

	public void SetRotation ( Quaternion newMotion )
		=> _currentMotion = newMotion;

	public void AddRotation ( Vector3 newMotion )
	{
		_currentMotion.x += newMotion.x;
		_currentMotion.y += newMotion.y;
		_currentMotion.z += newMotion.z;
	}
}
