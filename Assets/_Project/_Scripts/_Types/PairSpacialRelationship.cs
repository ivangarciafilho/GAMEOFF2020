using System;
using UnityEngine;

public interface IMonobehaviourOrigin
{
	MonoBehaviour origin { get; set; }
}

[Serializable]
public struct PairSpacialRelationship
{
	public MonoBehaviour origin { get; set; }

	public GameObject gameObjectA;
	public Transform transformA;
	public Vector3 positionA;
	public Vector3 directionToB;

	public GameObject gameObjectB;
	public Transform transformB;
	public Vector3 positionB;
	public Vector3 directionToA;

	public float distance;

	public PairSpacialRelationship ( MonoBehaviour origin, GameObject gameObjectB)
	{
		this.origin = origin;

		this.gameObjectA = origin.gameObject;
		this.transformA = origin.transform;
		this.gameObjectB = gameObjectB;
		this.transformB = gameObjectB.transform;

		//Ignored
		var vector3Zero = Vector3.zero;
		this.positionA = vector3Zero;
		this.positionB = vector3Zero;
		this.directionToA = vector3Zero;
		this.directionToB = vector3Zero;
		this.distance = 0;
	}

	public PairSpacialRelationship ( GameObject gameObjectA, GameObject gameObjectB )
	{
		this.gameObjectA = gameObjectA;
		this.transformA = gameObjectA.transform;
		this.gameObjectB = gameObjectB;
		this.transformB = gameObjectB.transform;

		//Ignored
		var vector3Zero = Vector3.zero;
		this.origin = null;
		this.positionA = vector3Zero;
		this.positionB = vector3Zero;
		this.directionToA = vector3Zero;
		this.directionToB = vector3Zero;
		this.distance = 0;
	}

	public PairSpacialRelationship ( Transform transformA, Transform transformB )
	{
		this.transformA = transformA;
		this.gameObjectA = transformA.gameObject;

		this.transformB = transformB;
		this.gameObjectB = transformB.gameObject;

		//Ignored
		var vector3Zero = Vector3.zero;
		this.origin = null;
		this.positionA = vector3Zero;
		this.positionB = vector3Zero;
		this.directionToA = vector3Zero;
		this.directionToB = vector3Zero;
		this.distance = 0;
	}

	public PairSpacialRelationship (MonoBehaviour origin, GameObject gameObjectA, Transform transformA, Vector3 positionA, Vector3 directionToB, GameObject gameObjectB, Transform transformB, Vector3 positionB, Vector3 directionToA, float distance)
	{
		this.origin = origin;
		this.gameObjectA = gameObjectA;
		this.transformA = transformA;
		this.positionA = positionA;
		this.directionToB = directionToB;
		this.gameObjectB = gameObjectB;
		this.transformB = transformB;
		this.positionB = positionB;
		this.directionToA = directionToA;
		this.distance = distance;
	}

	public  void SetPair ( Transform entityA, Transform entityB )
	{
		SetA ( entityA );
		SetB ( entityB );
	}

	public  void SetPair ( GameObject entityA, GameObject entityB )
	{
		SetA ( entityA );
		SetB ( entityB );
	}

	public  void SetPair ( GameObject entityA, Transform entityB )
	{
		SetA ( entityA );
		SetB ( entityB );
	}

	public  void SetPair ( Transform entityA, GameObject entityB )
	{
		SetA ( entityA );
		SetB ( entityB );
	}

	public void SetA ( Transform transformA)
		=> this.gameObjectA  = ( this.transformA = transformA).gameObject;

	public void SetB ( Transform transformB ) 
		=> this.gameObjectB = ( this.transformB = transformB).gameObject;

	public void SetA ( GameObject gameobjectA)
		=> this.transformA = (  this.gameObjectA = gameobjectA).transform;

	public void SetB ( GameObject gameobjectB)
		=> this.transformB = ( this.gameObjectB = gameobjectB).transform;

	public void SetSpatialRelationShip ( GameObject gameObjectA, GameObject gameObjectB )
	{
		SetPair ( gameObjectA, gameObjectB );
		UpdateSpacialRelationship ( );
	}

	public  void SetSpatialRelationShip ( Transform transformA, Transform transformB ) 
	{
		SetPair ( transformA, transformB );
		UpdateSpacialRelationship ( );
	}

	public void SetSpatialRelationShip ( GameObject gameObjectA, Transform transformB )
	{
		SetPair ( gameObjectA, transformB );
		UpdateSpacialRelationship ( );
	}

	public  void SetSpatialRelationShip ( Transform transformA, GameObject gameObjectB ) 
	{ 
		SetPair ( transformA, gameObjectB );
		UpdateSpacialRelationship ( );
	}

	public void UpdateSpacialRelationship ( )
	{
		positionA = transformA.position;
		positionB = transformB.position;
		distance = positionA.DistanceTo( positionB );

		directionToA = positionB.DirectionTo ( positionA );
		directionToB = -directionToA;
	}
}
