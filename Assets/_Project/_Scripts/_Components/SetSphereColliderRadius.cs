using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSphereColliderRadius : MonoBehaviour
{
	public SphereCollider itsSphereCollider;

	public void SetRadius ( float radius)
		=> itsSphereCollider.radius = radius;

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( itsSphereCollider == null )
			itsSphereCollider = GetComponent<SphereCollider> ( );
	}
#endif
}
