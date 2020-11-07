using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public Bullet bullet;
	public Transform firePoint;
	public float shootRate = 0.25f;
	float shootRateElapsed = 0.0f;
	
    void Start()
    {
	    shootRateElapsed = Time.time + shootRate;
    }

    void Update()
    {
	    Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
	    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
	    
	    transform.rotation = Quaternion.Euler(0, 0, angle);
	    
	    if(Input.GetButtonDown("Fire1") && shootRateElapsed < Time.time)
	    {
	    	var bulletInstance = Instantiate(bullet, firePoint.position, firePoint.rotation);
	    	
	    	shootRateElapsed = Time.time + shootRate;
	    }
    }
    
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if(firePoint != null)
		{
		Gizmos.color = Color.red;
			Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right);
		}
	}
}
