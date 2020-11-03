using System;
using NaughtyAttributes;
using UnityEngine;


public class FireEventOnTriggerEnter : MonoBehaviour
{
	public bool useTagFiltering;
	[ShowIf("useTagFiltering")] [Tag] public string tagFiltering;
	
	public bool useLayerFiltering;
	[ShowIf("useLayerFiltering")] public LayerMask layerFiltering;
	
	public TriggerEvent onTriggerEnter;

	public PairSpacialRelationship lastEventData;

	private void OnTriggerEnter ( Collider other )
	{
		// triggering Entity fits	 the layerFiltering?
		if ( useLayerFiltering )
			if ( layerFiltering == ( layerFiltering | ( 1 << other.gameObject.layer ) ) == false ) return;

		// triggering Entity fits the tagFiltering?
		if ( useTagFiltering )
			if (other.CompareTag(tagFiltering) == false ) return;

		lastEventData.SetSpatialRelationShip ( transform, other.transform);

		onTriggerEnter.Invoke ( ColliderEventType.Enter, other, lastEventData);
	}
}
