using System;
using NaughtyAttributes;
using UnityEngine;
using UltEvents;

[Serializable]
public class ScannerController
{
	[HorizontalLine(1)]//-----------------------------------------------------------------------------

	public float autoDisposeTime;
	private float disposeTime;

	public Transform positionConstraint;

	[HorizontalLine(1)]//-----------------------------------------------------------------------------

	[Layer]public int scannerLayer;

	public bool useLayerFiltering;
	[ShowIf("useLayerFiltering"), AllowNesting] 
	public LayerMask layerFiltering;

	public bool useTagFiltering;
	[ShowIf("useTagFiltering"), Tag, AllowNesting] 
	public string tagFiltering;

	[HorizontalLine ( 1 )]//-----------------------------------------------------------------------------

	public Vector2 radiusRange;
	public Vector2 animationLengthRange;
	[CurveRange(0,0,1,1)] public AnimationCurve radiusOverTime;
	public CollisionDetectionMode collisionDetectionMode;

	public bool setEventsReferences;

	public ScannerUpdateEvent onScannerUpdate;
	private Action<Scanner, float, float> onScannerLoopCallback;

	public ScannedEntityEvent onScannedEntity;
	private Action<Scanner, PairSpacialRelationship> onScannedEntityCallback;

	public ScannerEvent onScannerDispose;
	private Action<Scanner> onScannerDisposeCallback;

	public void Initialize ( )
	{
		if ( autoDisposeTime != 0 ) 
			disposeTime = Time.time 
				+ autoDisposeTime;
	}

	public void ProccesssProfileLoop (Scanner scanner, float time, float fixedDeltatime)
	{
		onScannerLoopCallback?.Invoke ( scanner, time, fixedDeltatime );
		if ( onScannerUpdate.HasCalls ) onScannerUpdate.Invoke (scanner, time, fixedDeltatime );
		if ( autoDisposeTime != 0 && time > autoDisposeTime ) DisposeScanner ( scanner );
	}

	public void ProccessScannedEntity ( Scanner scanner,  PairSpacialRelationship entity)
	{
		onScannedEntityCallback?.Invoke ( scanner, entity);
		if ( onScannedEntity.HasCalls ) onScannedEntity.Invoke (scanner, entity);
	}

	private void DisposeScanner (Scanner scanner)
	{
		onScannerDispose?.Invoke ( scanner );
		if ( onScannerDispose.HasCalls ) onScannerDispose.Invoke ( scanner );
	}
}

[Serializable] public class ScannerEvent : UltEvent< Scanner > { }
[Serializable] public class ScannedEntityEvent : UltEvent<Scanner, PairSpacialRelationship> { }
[Serializable] public class ScannerUpdateEvent : UltEvent<Scanner, float, float> { }

[Serializable]
public struct ProjectileProfile
{
	public float alignmentThreshold;

	[CurveRange(0,-1,1,1)]
	public AnimationCurve aimingSpeedCurve;
	public Vector2 aimingSpeedScaleRange;

	[ReadOnly, SerializeField] public float generatedAimingSpeedScale;
	[ReadOnly, SerializeField] public float currentAimingSpeed;

	[HorizontalLine ( 1 )]//-----------------------------------------------------------------------------

	[CurveRange(0,-1,1,1)]
	public AnimationCurve velocityCurve;
	public Vector2 velocityScaleRange;

	[ReadOnly, SerializeField] public float generatedVelocityScale;
	[ReadOnly, SerializeField] public float currentVelocity;

	[HorizontalLine ( 1 )]//-----------------------------------------------------------------------------


	public AnimationCurve driftingWeightCurve;
	public Vector2 driftingWeightScaleRange;

	[ReadOnly, SerializeField] public float generatedDriftingWeightScale;
	[ReadOnly, SerializeField] public float currentTrajectoryFixingWeight;

	[HorizontalLine ( 1 )]//-----------------------------------------------------------------------------

	public Vector2 projectileLifetimeRange;
	[ReadOnly, SerializeField] public float generatedProjectileLifetime;
}