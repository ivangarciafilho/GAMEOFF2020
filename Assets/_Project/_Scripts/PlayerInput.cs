using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public Ball ball;
	Vector3 dir;
	
	Vector3 mouseWorldPos;
	Vector3 originPoint;
	
	float force = 0.0f;
	bool canThrow = false;
	
    void Start()
    {
        
    }

    void Update()
	{
		mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		if(Input.GetButtonDown("Fire1"))
		{
			originPoint = mouseWorldPos;			
		}
    	
	    if(Input.GetButton("Fire1"))
	    {	    	
	    	dir = (originPoint - mouseWorldPos);
	    	
	    	Debug.DrawLine(ball.transform.position, ball.transform.position - (originPoint - mouseWorldPos), Color.magenta);
	    	
	    	force = GameUtils.Map(dir.magnitude, 0.0f, 10.0f, 0.0f, ball.maxForce);
	    }
	    
		if(Input.GetButtonUp("Fire1")) canThrow = true;
	}
    
	void FixedUpdate()
	{
		if(canThrow)
		{
			ball.Throw(dir.normalized, force);
			canThrow = false;
		}
	}
}
