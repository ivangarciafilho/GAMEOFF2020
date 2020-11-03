using NaughtyAttributes;
using UnityEngine;

public class FireEventOnReferencePoint : MonoBehaviour
{
	[BoxGroup ( "Parameters" )] public Transform referenceOverride;
	[BoxGroup ( "Parameters" )] public Space space = Space.World;

	[BoxGroup ( "OutputEvents" )] public Vector3Event passPosition;
	[BoxGroup ( "OutputEvents" )] public QuaternionEvent passRotation;
	[BoxGroup ( "OutputEvents" )] public TransformEvent passTransform;
	[BoxGroup ( "OutputEvents" )] public DataPackageEvent  passData;
	[BoxGroup ( "OutputEvents" )] public SpatialReferenceEvent triggeredEvents;
	


	private DataPackage dataPackage;

	private Transform reference 
	{
		get
		{
			return referenceOverride ? referenceOverride : transform;
		}
	}

	private (Vector3 position, Quaternion  rotation) coordinates
	{
		get
		{
			return 
				(space == Space.Self
				? reference.localPosition : reference.position,

				space == Space.Self
				? reference.localRotation: reference.rotation);
		}
	}


	private Vector3 currentPosition;
	private Quaternion currentRotation;
	private Transform currentReference;
	
	public void FireEvents ( )
	{
		currentReference = reference;
		currentPosition = coordinates.position;
		currentRotation = coordinates.rotation;

		if ( triggeredEvents.HasCalls ) triggeredEvents.
				   Invoke ( currentReference, space, currentPosition, currentRotation );

		if ( passData.HasCalls )
		{
			dataPackage.transformValue = currentReference;
			dataPackage.spaceValue = space;
			dataPackage.vector3Value = currentPosition;
			dataPackage.quaternionValue = currentRotation;
			dataPackage.monoBehaviourValue = this;
			dataPackage.gameObjectvalue = gameObject;

			passData.Invoke ( dataPackage );
		}

		if ( passPosition.HasCalls )
			passPosition.Invoke ( currentPosition );

		if ( passRotation.HasCalls )
			passRotation.Invoke ( currentRotation );

		if ( passTransform.HasCalls )
			passTransform.Invoke ( currentReference );
	}
}
