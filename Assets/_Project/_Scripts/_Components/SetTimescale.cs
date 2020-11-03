using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimescale : MonoBehaviour
{
	public float defaultValue;

	public void Set ( )=> Set ( defaultValue);

	public void Set ( float scale)
	{
		scale = Mathf.Clamp01 ( scale );
		Time.timeScale = scale;
	}
}
