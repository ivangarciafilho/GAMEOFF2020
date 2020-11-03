using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent ( typeof ( Rigidbody ) )]
[RequireComponent ( typeof ( SphereCollider ) )]
[RequireComponent ( typeof ( ScaleFloatOverTime ) )]
[RequireComponent ( typeof ( FireEventOnTriggerEnter ) )]
[RequireComponent ( typeof ( FireEventOnCollisionEnter ) )]
[RequireComponent ( typeof ( SetRigidbodyVelocity ) )]
[RequireComponent ( typeof ( SetRigidbodyPosition ) )]
[RequireComponent ( typeof ( PhysicsManager ) )]
[RequireComponent ( typeof ( FixedUpdateDispatcher ) )]
[RequireComponent ( typeof ( GenericPackageDispatcher ) )]
[RequireComponent ( typeof ( GenericMessageBroadcaster ) )]

public class Scanner : PooledBehaviour<Scanner>
{
	[BoxGroup ( "Dependencies" )] public bool showDependencies;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public Rigidbody itsRigidbody;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public SphereCollider itsSphereCollider;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public ScaleFloatOverTime sphereScaler;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public FireEventOnTriggerEnter itsTriggerSystem;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public FireEventOnCollisionEnter itsCollisionSystem;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public SetRigidbodyVelocity itsVelocityManager;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public SetRigidbodyPosition itsPositionManager;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public FixedUpdateDispatcher itsFixedUpdateDispatcher;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public PhysicsManager itsPhysicsManager;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public GenericPackageDispatcher itsPackageDispatcher;

	[ShowIf ( "showDependencies" ), BoxGroup ( "Dependencies" )]
	public GenericMessageBroadcaster itsMessageDispatcher;

	[BoxGroup ( "ScannerProfile" ), SerializeField]
	private ScannerController defaultProfile;

	[BoxGroup ( "ScannerProfile" ), SerializeField]
	private ScannerController currentProfile;

	private IScannerHandler currentScannerHandler;

	[BoxGroup ( "ScannedData" )]
	public List<PairSpacialRelationship> scannedEntities = new List<PairSpacialRelationship> ( );

	protected override SpawningData BeforeSpawning ( SpawningData spawnData )
	{
		spawnData = SetupScanner ( spawnData );
		return base.BeforeSpawning ( spawnData );
	}

	private SpawningData SetupScanner ( SpawningData spawningData )
	{
		scannedEntities.Clear ( );

		if ( spawningData.spawningObject is IScannerHandler )
		{
			currentScannerHandler = ( IScannerHandler ) spawningData.spawningObject;
		}
		else
		{
			currentScannerHandler = null;
		}

		if ( spawningData.customData != null
			&& spawningData.customData is ScannerController )
		{
			currentProfile = ( ScannerController ) spawningData.customData;
		}
		else
		{
			currentProfile = defaultProfile;
		}

		gameObject.layer = currentProfile.scannerLayer;
		itsTriggerSystem.tagFiltering = currentProfile.tagFiltering;
		itsTriggerSystem.layerFiltering = currentProfile.layerFiltering;

		if ( spawningData.parent )
		{
			if ( currentProfile.positionConstraint == null )
				currentProfile.positionConstraint = spawningData.parent;

			spawningData.parent = null;
		}

		itsPositionManager.SetPosition ( spawningData.position );
		itsPositionManager.follow = currentProfile.positionConstraint;

		sphereScaler.SetScaleMultiplier ( currentProfile.radiusRange );
		sphereScaler.SetScalingDelay ( currentProfile.animationLengthRange );
		sphereScaler.scalingCurve = currentProfile.radiusOverTime;

		return spawningData;
	}

	public void DispatchScanningResults (
		ColliderEventType eventType,
		Collider triggeringCollider,
		PairSpacialRelationship scannedEntity )
	{
		if ( currentScannerHandler != null )
			currentScannerHandler.OnEntityScanned ( this, scannedEntity );

		if ( scannedEntities.
				Select ( _entity => _entity.gameObjectB ).
				Contains ( scannedEntity.gameObjectB )
				== false )
		{
			scannedEntities.Add ( scannedEntity );
		}

		currentProfile.ProccessScannedEntity(this, scannedEntity);
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( itsSphereCollider == null )
			itsSphereCollider = GetComponent<SphereCollider> ( );

		if ( itsRigidbody == null )
			itsRigidbody = GetComponent<Rigidbody> ( );

		if ( itsPhysicsManager == null )
			itsPhysicsManager = GetComponent<PhysicsManager> ( );

		if ( sphereScaler == null )
			sphereScaler = GetComponent<ScaleFloatOverTime> ( );

		if ( itsTriggerSystem == null )
			itsTriggerSystem = GetComponent<FireEventOnTriggerEnter> ( );

		if ( itsCollisionSystem == null )
			itsCollisionSystem = GetComponent<FireEventOnCollisionEnter> ( );

		if ( itsVelocityManager == null )
			itsVelocityManager = GetComponent<SetRigidbodyVelocity> ( );

		if ( itsPositionManager == null )
			itsPositionManager = GetComponent<SetRigidbodyPosition> ( );

		if ( itsFixedUpdateDispatcher == null )
			itsFixedUpdateDispatcher = GetComponent<FixedUpdateDispatcher> ( );

		if ( itsPackageDispatcher == null )
			itsPackageDispatcher = GetComponent<GenericPackageDispatcher> ( );

		if ( itsMessageDispatcher == null )
			itsMessageDispatcher = GetComponent<GenericMessageBroadcaster> ( );
	}

#endif
}
