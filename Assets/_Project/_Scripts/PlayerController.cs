using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Rigidbody2D rb2D;
	public float moveSpeed;
	float horizontal;

	void Update () 
	{
		horizontal = Input.GetAxisRaw("Horizontal");
	}

	void FixedUpdate () 
	{		
		if(horizontal != 0.0f)
			rb2D.MovePosition(rb2D.position + (Vector2)transform.TransformDirection(new Vector3(horizontal, 0, 0)) * moveSpeed * Time.deltaTime);
	}
}