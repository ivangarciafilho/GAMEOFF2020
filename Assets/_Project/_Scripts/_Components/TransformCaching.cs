using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class TransformCaching : MonoBehaviour
{
	public Vector3 localPosition;
	public Vector3 previousLocalPosition;

	public Vector3 position;
	public Vector3 previousPosition;

	public Quaternion localRotation;
	public Quaternion previousLocalRotation;

	public Quaternion rotation;
	public Quaternion previousRotation;

	public Vector3 previousForward;
	public Vector3 forward;

	private void Update ( )
	{
		previousLocalPosition = localPosition;
		localPosition = transform.localPosition;

		previousPosition = position;
		position = transform.position;

		previousLocalRotation = localRotation;
		localRotation = transform.rotation;

		previousRotation = rotation;
		rotation = transform.rotation;

		previousForward = forward;
		forward = transform.forward;

	}
}
