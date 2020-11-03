using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FireEventOnTriggerExit : MonoBehaviour
{
	public bool useTagFiltering;
	[ShowIf("useTagFiltering")] [Tag] public string tagFiltering;
	
	public bool useLayerFiltering;
	[ShowIf("useLayerFiltering")] public LayerMask layerFiltering;
	
	public TriggerEvent onTriggerEnter;

	public PairSpacialRelationship lastEventData;

	private void OnTriggerExit ( Collider other )
	{
	   // triggering Entity fits	 the layerFiltering?
		if ( useLayerFiltering )
			if ( layerFiltering == ( layerFiltering | ( 1 << other.gameObject.layer ) ) == false ) return;

		// triggering Entity fits the tagFiltering?
		if ( useTagFiltering )
			if (other.CompareTag(tagFiltering) == false ) return;

		lastEventData.gameObjectA = gameObject;
		lastEventData.transformA= transform;
		lastEventData.gameObjectB = other.gameObject;
		lastEventData.transformB = other.transform;

		lastEventData.UpdateSpacialRelationship ( );

		onTriggerEnter.Invoke ( ColliderEventType.Exit, other, lastEventData);
	}
}
