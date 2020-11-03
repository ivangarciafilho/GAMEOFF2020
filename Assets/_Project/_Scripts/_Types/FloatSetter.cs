using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FloatSetter : MonoBehaviour
{
	public float defaultValue;

	public void Set ( )=> Set ( defaultValue);

	public abstract void Set ( float scale );
}