using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct SpawningData
{
	public Object spawningObject;
	public Vector3 position;
	public Quaternion rotation;
	public System.Object customData;
	public Transform parent;
	public bool toggle;
}
