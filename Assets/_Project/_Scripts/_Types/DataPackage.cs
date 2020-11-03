using System;
using System.Collections;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;
using Object = System.Object;

[Serializable]
public struct DataPackage
{
	[HorizontalLine(2)] //--------------------------------

	public Object objectValue;
	public GameObject gameObjectvalue;
	public MonoBehaviour monoBehaviourValue;
	public Transform transformValue;
	public Component componentValue;
	 public IList listValue;

	[HorizontalLine(2)] //--------------------------------

	public bool boolValue;
	public float floatValue;
	public int intValue;
	public string stringValue;
	public Vector3 vector3Value;
	public Quaternion quaternionValue;
	public Space spaceValue;
	[CurveRange(0,-1,1,1)] public AnimationCurve animationCurveValue;

	[HorizontalLine(2)] //--------------------------------

	public Rigidbody rigidbodyValue;
	public Collider colliderValue;
	public Collision collisionValue;
	public Ray rayValue;
	 public RaycastHit hitValue;

	[HorizontalLine ( 2 )] //--------------------------------
	public UltEvent<DataPackage> triggeredEvent;
	public Action<DataPackage> triggeredAction;
}