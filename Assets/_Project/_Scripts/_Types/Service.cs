using UnityEngine;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
public class Service<T> : MonoBehaviour where T : Component
{
    [SerializeField] private bool dontDestroyOnLoad = false;

    private static T _instance;
    public static T instance => _instance;

    protected virtual void Awake ( )
    {
        if ( _instance != null && _instance != this )
        {
#if UNITY_EDITOR
            Debug.Log (gameObject.name + ", is a redundant instance of singleton: " + this.GetType().Name);
#endif
	        DestroyImmediate ( gameObject );
	        return;
        }  

	    _instance = this as T;
        
                
	    if(dontDestroyOnLoad)
	    {
	    	DontDestroyOnLoad(_instance);
	    }
          
    }
}

