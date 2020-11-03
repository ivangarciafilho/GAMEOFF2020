using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloodstone;
using UnityEngine;

public class TurnOnRandomChild : MonoBehaviour
{
	public GameObject [ ] childs;
	[ReadOnly, SerializeField] private int childCount;
	[ReadOnly, SerializeField] private MeshRenderer [ ] meshRenderers;

	private int chosenChildIndex = 0;
	public void ActivateRandomChid ( )
	{
		chosenChildIndex = Random.Range ( 0, childCount);

		for ( int i = 0; i < childCount; i++ )
			meshRenderers [ i ].enabled = (i == chosenChildIndex);
	}


#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( childs == null ) 
			childs = new GameObject [ 0 ];

		childs = childs.
			Where ( _item => _item != null).
			ToArray();

		childCount = childs.Length;

		meshRenderers = childs.
			Select ( _child => _child.GetComponent<MeshRenderer>()).
			ToArray();
	}
#endif
}
