using System.Linq;
using Bloodstone;
using NaughtyAttributes;
using UnityEngine;
using ReadOnly = Bloodstone.ReadOnlyAttribute;

public class CollectEnititiesWithinRange : MonoBehaviour
{
	[BoxGroup("ScanningParameters")] public Transform centerOverride;
	[BoxGroup("ScanningParameters")] public LayerMask layerMask;
	[BoxGroup("ScanningParameters")] public float radius;
	
	[BoxGroup("PrimaryCallbacks")] public ColliderEvent passCollider;
	[BoxGroup("PrimaryCallbacks")] public ColliderEvent_Array passColliders;
	[HideInInspector] public Collider[] collectedColliders;

	public bool hasColliderCallbacks 
		=> passCollider.HasCalls || passColliders.HasCalls;

	[BoxGroup("PrimaryCallbacks")] public SpacialRelationshipEvent passSpatialRelationship;
	[BoxGroup("PrimaryCallbacks")] public SpacialRelationshipEvent_Array passSpatialRelationships;
	public bool hasSpatialRelationshipCallbacks 
		=> passSpatialRelationship.HasCalls || passSpatialRelationships.HasCalls;

	[BoxGroup("SecondaryCallbacks")] public GameobjectEvent passGameobject;
	[BoxGroup("SecondaryCallbacks")] public GameobjectEvent_Array passGameobjects;
	[HideInInspector] public GameObject[] collectedGameobjects;

	public bool hasGameobjectCallbacks
		=> passGameobject.HasCalls || passGameobjects.HasCalls;

	[BoxGroup("SecondaryCallbacks")] public TransformEvent passTransform;
	[BoxGroup("SecondaryCallbacks")] public TransformEvent_Array passTransforms;
	[HideInInspector] public Transform[] collectedTransforms;

	public bool hasTransformCallbacks
		=> passTransform.HasCalls || passTransforms.HasCalls;

	[BoxGroup("SecondaryCallbacks")] public Vector3Event passPosition;
	[BoxGroup("SecondaryCallbacks")] public Vector3Event_Array passPositions;
	[HideInInspector] public Vector3[] collectedPositions;

	public bool hasPositionCallbacks
		=> passPosition.HasCalls || passPositions.HasCalls;

	[BoxGroup("SecondaryCallbacks")] public DataPackageEvent passDataPackage;
	[BoxGroup("SecondaryCallbacks")] public DataPackageEvent_Array passDataPackages;
	[HideInInspector] public DataPackage[] collectedDataPackages;

	public bool hasDataPackageCallbacks 
		=> passDataPackage.HasCalls || passDataPackages.HasCalls;

	[BoxGroup("ScanningResult"), ReadOnly] public int amountCollected;
	[BoxGroup ( "ScanningResult" ), ReadOnly, SerializeField]
	public PairSpacialRelationship [ ] collectedSpatialRelationships;

	public Vector3 center 
	{ 
		get 
		{ 
			return centerOverride 
				? centerOverride.transform.position : transform.position; 
		}
	}

	public void Reset ( )
	{
		amountCollected = 0;

		collectedSpatialRelationships
		= new PairSpacialRelationship [ 0 ];
	}

	public PairSpacialRelationship[] Collect ( )
	{
		collectedColliders = Physics.OverlapSphere (center, radius, layerMask);
		amountCollected = collectedColliders.Length;

		if (hasSpatialRelationshipCallbacks )
			collectedSpatialRelationships  = new PairSpacialRelationship [ amountCollected ];
			
		if ( hasDataPackageCallbacks )
			collectedDataPackages = new DataPackage[amountCollected];

		if ( hasGameobjectCallbacks)
			collectedGameobjects = new GameObject [ amountCollected ];

		if ( hasTransformCallbacks )
			collectedTransforms = new Transform [ amountCollected ];

		if ( hasPositionCallbacks )
			collectedPositions = new Vector3 [ amountCollected ];

		if ( amountCollected > 0 )
		{
			for ( int i = 0; i < amountCollected; i++ )
			{
				if ( hasSpatialRelationshipCallbacks )
				{
					collectedSpatialRelationships [ i ] = 
						new PairSpacialRelationship 
					{ 
						origin = this, 

						gameObjectA = gameObject,
						transformA = transform,

						gameObjectB = collectedColliders [ i ].gameObject,
						transformB = collectedColliders [ i ].transform
					};

					if ( passSpatialRelationship.HasCalls )
						passSpatialRelationship.Invoke ( collectedSpatialRelationships[i] );
				}

				if ( hasDataPackageCallbacks )
				{
					collectedDataPackages [ i ] = new DataPackage
					{
						monoBehaviourValue = this,
						colliderValue = collectedColliders [ i ],
						transformValue = collectedColliders [ i ].transform,
						vector3Value = collectedColliders [ i ].transform.position
					};

					if ( passDataPackage.HasCalls )
					passDataPackage.Invoke ( collectedDataPackages[i]);
				}

				if ( passCollider.HasCalls )
					passCollider.Invoke ( collectedColliders[i] );

				if ( hasGameobjectCallbacks )
				{
					collectedGameobjects [ i ] = collectedColliders [ i ].gameObject;

					if ( passGameobject.HasCalls ) 
						passGameobject.Invoke ( collectedGameobjects[i] );
				}

				if ( hasTransformCallbacks )
				{
					collectedTransforms [ i ] = collectedColliders [ i ].transform;

					if ( passTransform.HasCalls ) 
					passTransform.Invoke ( collectedTransforms[i] );
				}

				if ( hasPositionCallbacks )
				{
					collectedPositions [ i ] = collectedColliders [ i ].transform.position;

					if ( passPosition.HasCalls )
					passPosition.Invoke ( collectedPositions[i] );
				}
			}

			if ( passSpatialRelationships.HasCalls ) 
				passSpatialRelationships.Invoke ( collectedSpatialRelationships );

			if ( passDataPackages.HasCalls )
				passDataPackages.Invoke (collectedDataPackages);

			if ( passColliders.HasCalls )
				passColliders.Invoke ( collectedColliders);

			if ( passGameobjects.HasCalls )
				passGameobjects.Invoke ( collectedGameobjects );

			if ( passTransforms.HasCalls )
				passTransforms.Invoke ( collectedTransforms );

			if ( passPositions.HasCalls )
				passPositions.Invoke ( collectedPositions );
		}

		return collectedSpatialRelationships;
	}

	public PairSpacialRelationship[] entitiesWithinRange
	{
		get
		{
			collectedSpatialRelationships = collectedSpatialRelationships.
				Where ( _entity => _entity.gameObjectB != null ).
				ToArray();

			var amountOfAvailableEntities 
				= collectedSpatialRelationships.Length;

			for ( int i = 0; i < amountOfAvailableEntities; i++ )
				collectedSpatialRelationships [ i ].UpdateSpacialRelationship ( );

			collectedSpatialRelationships = collectedSpatialRelationships.
				Where ( _entity => _entity.distance < radius ).
				ToArray ( );

			if ( amountOfAvailableEntities > 1 )
			{
				collectedSpatialRelationships = collectedSpatialRelationships.
				OrderBy ( _entity => _entity.distance ).
				ToArray ( );
			}

			return collectedSpatialRelationships;
		}
	}
}
