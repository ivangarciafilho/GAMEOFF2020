using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 10.0f;

	void Start()
	{
		Destroy(gameObject, 2.0f);
	}

    void Update()
    {
	    transform.position += transform.right * speed * Time.deltaTime;
    }
}
