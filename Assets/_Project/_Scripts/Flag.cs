using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flag : MonoBehaviour
{
	public UnityEvent OnReachThisGoal = new UnityEvent();
	
	public SpriteRenderer spriteRend;
	public Sprite spriteDown;
	
	bool checkForSpeed;
	
	static Flag currentFlag;

    void Start()
    {
        
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		if(currentFlag != null)
		{
			GameManager.instance.targetGroup.RemoveMember(currentFlag.transform);
		}
		
		currentFlag = this;
		
		GameManager.instance.targetGroup.AddMember(transform, 0.6f, 0);
	}

    void Update()
    {
	    if(checkForSpeed)
	    {
	    	if(GameManager.instance.ball.rb2D.velocity.magnitude < 0.1f)
	    	{
		    	gameObject.SetActive(false);
		    	OnReachThisGoal.Invoke();
		    	
		    	spriteRend.sprite = spriteDown;
		    	
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
