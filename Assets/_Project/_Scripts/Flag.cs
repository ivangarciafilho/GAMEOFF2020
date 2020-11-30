using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flag : MonoBehaviour
{
	public UnityEvent OnReachThisGoal = new UnityEvent();
	
	bool checkForSpeed;

    void Start()
    {
        
    }

    void Update()
    {
	    if(checkForSpeed)
	    {
	    	if(GameManager.instance.ball.rb2D.velocity.magnitude < 0.1f)
	    	{
		    	gameObject.SetActive(false);
		    	OnReachThisGoal.Invoke();
		    	
		    	checkForSpeed = false;
	    	}
	    }
    }

	protected void OnTriggerEnter2D(Collider2D other)
	{
		GameManager.instance.BallHitAFlag(transform);
		checkForSpeed = true;
	}
}
