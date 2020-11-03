using UnityEngine;


[RequireComponent(typeof( SetRigidbodyVelocity  ))]
[RequireComponent ( typeof ( EntitiesWithinRangeStorage ) )]
public class Attractable : MonoBehaviour
{
	public bool automaticallyAttract;
	public bool onlyByAttractors;
	public float attractionForce;
	public SetRigidbodyVelocity itsMoveRigidbody;
	public EntitiesWithinRangeStorage itsCacheNearby;


	public void UpdateAttractionForce ( )
	{
		
	}
}
