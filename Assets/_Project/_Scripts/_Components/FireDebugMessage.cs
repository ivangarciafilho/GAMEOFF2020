using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDebugMessage : MonoBehaviour
{
#if UNITY_EDITOR
	public string message = "within";
	public void Fire ( ) { Debug.Log ( "within" ); }
#endif
}
