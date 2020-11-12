using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityAttractor : MonoBehaviour {

	//public float distanceField = 10.0f;
	public static List<GravityAttractor> attractors = new List<GravityAttractor>();

	public float gravity = -12;

	void Awake()
	{
		attractors.Add(this);
	}

	public void Attract(Transform body, float offsetGravity = 0.0f) 
	{
		
		
			Vector3 gravityUp = (body.position - transform.position).normalized;
			Vector3 localUp = body.up;
	
		Debug.Log(gravity+ offsetGravity);
	
		body.GetComponent<Rigidbody2D>().AddForce(gravityUp * (gravity + offsetGravity));
			
			float ang = Mathf.Atan2(gravityUp.y, gravityUp.x) * Mathf.Rad2Deg;
			ang -= 90.0f;
	
			Quaternion targetRotation = Quaternion.Euler(0, 0, ang);
				body.rotation = Quaternion.Slerp(body.rotation,targetRotation, 50f * Time.deltaTime );
		
	}
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		//Gizmos.color = Color.cyan;
		//Gizmos.DrawWireSphere(transform.position, distanceField);
	}
}