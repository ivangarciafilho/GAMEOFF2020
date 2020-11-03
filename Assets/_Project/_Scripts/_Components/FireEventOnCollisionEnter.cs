using NaughtyAttributes;
using UnityEngine;

public class FireEventOnCollisionEnter : MonoBehaviour
{
	public bool useLayerFiltering;
	[ShowIf("useLayerFiltering")] public LayerMask layerFiltering;

	public bool useTagFiltering;
	[ShowIf("useLayerFiltering"),Tag] public string tagFiltering;

	public CollisionEvent onCollisionEnter;

	public Collision lastCollisionData;

	private void OnCollisionEnter ( Collision collision )
	{
		var otherGameobject = collision.gameObject;
		var otherCollider = collision.collider;

		if ( useLayerFiltering )
			if ( layerFiltering == ( layerFiltering | ( 1 << otherGameobject.layer ) ) == false ) 
				return;

		if ( useTagFiltering )
			if (collision.gameObject.CompareTag(tagFiltering) == false ) return;

		lastCollisionData = collision;
		onCollisionEnter.Invoke ( ColliderEventType.Enter, lastCollisionData);
	}
}
