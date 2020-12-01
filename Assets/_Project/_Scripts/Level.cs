using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public Transform firstBallPosition;
	public GameObject[] flags;
	public Material skybox;
	public Transform firstWorld;
	
	public void Setup()
    {
	    GameManager.instance.ball.transform.position = firstBallPosition.position;
	    foreach(var flag in flags)
	    {
	    	flag.SetActive(false);
	    	
	    	Flag f = flag.GetComponentInChildren<Flag>();
	    	f.gameObject.SetActive(true);
	    }
	    
	    flags[0].SetActive(true);
	    
	    RenderSettings.skybox = skybox;
    }

	void OnEnable()
	{
		
	}

    void Update()
    {
        
    }
}
