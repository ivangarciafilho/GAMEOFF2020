using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class GravityBody : MonoBehaviour 
{
	public float custtomOffsetGravity = 0.0f;
	Rigidbody2D rb2D;
	Transform myTransform;
	
	public GravityAttractor attractor { get; private set; }

	void Start () 
	{
		rb2D = GetComponent<Rigidbody2D>();
		
		rb2D.gravityScale = 0;
		rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

		myTransform = transform;
	}

	void FixedUpdate () 
	{
		GravityAttractor closestAttractor = null;
		float closestMag = 1000000000;
	
		for(int i = 0; i < GravityAttractor.attractors.Count; i++)
		{
			var attractor = GravityAttractor.attractors[i];
			if(!attractor.gameObject.activeInHierarchy) continue;
			
			float mag = (attractor.transform.position - transform.position).magnitude;
			if(attractor.useDistanceField) // if use distance field 
			{
				if(mag > attractor.distanceField) // if distance if bigger already than the distanceField, discard it
					continue;
			}
			
			
			if(mag < closestMag)
			{
				closestAttractor = attractor;
				closestMag = mag;
			}
		}
		
		attractor = closestAttractor;
		
		if(closestAttractor != null)
			closestAttractor.Attract(myTransform, custtomOffsetGravity);
	}
}