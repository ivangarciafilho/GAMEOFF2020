using Bloodstone;
using UnityEngine;


public class WorldToLocalVectorConversion : MonoBehaviour
{
	public Transform spaceReferenceOverride;
	public Vector3ValueEvent onConvert;

	[ReadOnly, SerializeField] private Vector3 _valueToLocal;
	public Vector3 valueToLocal => _valueToLocal;

	public void Convert ( Vector3 value )
	{
		_valueToLocal = spaceReferenceOverride ?
			spaceReferenceOverride.rotation * value
			: transform.rotation * value;

		if ( onConvert.HasCalls ) onConvert.Invoke ( value );
	}
}
