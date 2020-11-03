using UnityEngine;

public class SetTransformScale : MonoBehaviour
{
	public Transform transformOverride;
	public float scaleMultiplier = 2;
	public Vector3 value;

	private Transform targetTransform 
	{
		get
		{
			return transformOverride ? 
				transformOverride : transform;
		}
	}

	public void Scale (float uniformScale )
		=>targetTransform.localScale = Vector3.one * (uniformScale * scaleMultiplier);

	public void Scale ( )
		=>targetTransform.localScale = value;

	public void Scale ( Vector3 valueOverride)
		=>targetTransform.localScale = valueOverride;
}
