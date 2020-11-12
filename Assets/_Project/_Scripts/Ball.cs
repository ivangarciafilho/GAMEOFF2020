using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public GravityBody body;
	public Rigidbody2D rb2D;
	public float maxForce = 600.0f;
 
	float targetCustomGravity = -10.0f;
 
    void Start()
    {
        
    }
    
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		body.custtomOffsetGravity = Mathf.Lerp(body.custtomOffsetGravity, -25.0f, Time.deltaTime * 0.9f);
	}
    
	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	protected void FixedUpdate()
	{
		var velocity = rb2D.velocity;
		velocity *= 0.99f;
		rb2D.velocity = velocity;
	}

	public void Throw(Vector2 dir, float force)
    {
		dir *= force;
	    rb2D.AddForce(dir);
		
	    body.custtomOffsetGravity = 0.0f;
    }
}
