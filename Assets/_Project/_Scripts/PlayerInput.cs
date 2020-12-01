using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public Ball ball;
	public LineRenderer lineInput;
	Vector3 dir;
	
	Vector3 mouseWorldPos;
	Vector3 originPoint;
	
	float force = 0.0f;
	bool canThrow = false;
	
    void Start()
    {
	    lineInput.enabled = false;
    }

    void Update()
	{
		mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		if(Input.GetButtonDown("Fire1"))
		{
			originPoint = mouseWorldPos;	
			lineInput.enabled = true;
		}
    	
	    if(Input.GetButton("Fire1"))
	    {	    	
	    	dir = (originPoint - mouseWorldPos);
	    	
	    	Debug.DrawLine(ball.transform.position, ball.transform.position - (originPoint - mouseWorldPos), Color.magenta);
	    	
	    	force = GameUtils.Map(dir.magnitude, 0.0f, 10.0f, 0.0f, ball.maxForce);
	    	
	    	Vector3[] linePoints = new Vector3[lineInput.positionCount];
	    	lineInput.GetPositions(linePoints);
	    	
		    linePoints[1] = ball.transform.position + dir;
		    linePoints[0] = (ball.transform.position - (originPoint - mouseWorldPos)) + dir;
		    
		    Vector3 dirBetweenTwoPoints = (linePoints[1] - linePoints[0]).normalized;
		    
		    for(int i = 0; i < linePoints.Length; i++)
		    	linePoints[i] += dirBetweenTwoPoints;
		    
		    lineInput.SetPositions(linePoints);
	    }
	    
		if(Input.GetButtonUp("Fire1")) 
		{
			canThrow = true;
			lineInput.enabled = false;
		}
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		lineInput.enabled = false;
	}
    
	void FixedUpdate()
	{
		if(canThrow)
		{
			ball.Throw(dir.normalized, force);
			canThrow = false;
			
			gameObject.SetActive(false);
		}
	}
}
