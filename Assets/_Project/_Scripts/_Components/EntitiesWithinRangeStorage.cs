using System.Linq;
using Bloodstone;
using UnityEngine;

public class EntitiesWithinRangeStorage : MonoBehaviour
{
	public Transform centerOverride = null;
	public bool allowCopies = false;
	public float radius;

	[ReadOnly, SerializeField] private PairSpacialRelationship[] storedEntities 
		= new PairSpacialRelationship [0];

	[ReadOnly, SerializeField] private int  _amountOfEntitiesStored = 0;
	public int  amountOfEntitiesStored 
		=> _amountOfEntitiesStored ;

	private Vector3 center { get { return centerOverride == null ? transform.position : centerOverride.position; } }
	
	public void StoreEntities ( GameObject[] entities)
	{
		var amountOfEntities = entities.Length;

		for ( int i = 0; i < amountOfEntities; i++ )
			StoreEntity ( entities[i]);
	}

	public void StoreEntities ( PairSpacialRelationship[] entities)
	{
		var amountOfEntities = entities.Length;

		for ( int i = 0; i < amountOfEntities; i++ )
			StoreEntity ( entities[i] );
	}

	

	//For collisions
	public void StoreEntity (ColliderEventType triggeringMotion, Collision triggeringEntity)
		=> StoreEntity ( triggeringEntity.gameObject );

	//For triggers
	public void StoreEntity (ColliderEventType triggeringMotion, Collider  triggeringEntity, PairSpacialRelationship entity)
		=> StoreEntity ( entity );

	//For General  Usage
	public bool StoreEntity ( GameObject entity)
		=> StoreEntity (  GeneratePairSpatialRelationship(entity) );

	//Main
	public bool StoreEntity ( PairSpacialRelationship newPairSpatialRelationship)
	{
		if ( !allowCopies && storedEntities.
			Any(_entity => _entity.gameObjectB ==  newPairSpatialRelationship.gameObjectB ) ) 
			return  false;

		var currentEntitiesNearby = storedEntities.
			ToList ( );

		currentEntitiesNearby.
			Add ( newPairSpatialRelationship);

		storedEntities = currentEntitiesNearby.
			ToArray ( );

		return true;
	}

	public PairSpacialRelationship[]  EntitiesWithinRange
	{
		get
		{
			var validEntities = storedEntities.
				Where (_entity => _entity.gameObjectB != null).
				ToArray();

			var currentAmountOfEntities = validEntities.Length;
			for ( int i = 0; i < currentAmountOfEntities; i++ )
				validEntities [ i ].UpdateSpacialRelationship();

			validEntities = validEntities.
				Where (_entity => _entity.distance < radius ).
				ToArray();

			storedEntities = validEntities.
				OrderBy(_entity => _entity.distance).
				ToArray ( );

			return storedEntities;
		}
	}

	private  PairSpacialRelationship GeneratePairSpatialRelationship ( GameObject entity)
	{
		var newPairSpacialRelationship = 
				new PairSpacialRelationship 
				{ 
					origin = this, 

					gameObjectA = gameObject,
					transformA = transform,

					gameObjectB = entity,
					transformB = entity.transform
				};

			newPairSpacialRelationship.UpdateSpacialRelationship ( );

		return newPairSpacialRelationship;
	}

	public void Reset ( )
		=> storedEntities = new  PairSpacialRelationship[0];
}
