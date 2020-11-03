using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
public class PooledBehaviour<T> : MonoBehaviour where T : Component
{
	[BoxGroup ( "PoolingParameters" ), SerializeField, ReadOnly] private SpawningData _spawningData;
	public SpawningData  dataUsedToSpawn => _spawningData;

	protected static int fence { get; private set; }
	public int dynamicIndex { get; private set; }

	protected static List<PooledBehaviour<T>> instances { get; } = new List<PooledBehaviour<T>> ( 30 );
	public static List<PooledBehaviour<T>> currentInstances { get { return instances; } }
	public static int amountOfInstances { get { return instances.Count; } }
	public static bool available { get { return fence < instances.Count; } }

	[BoxGroup ( "PoolingParameters" ), SerializeField] private SpawningDataEvent _beforeSpawning = null;
	public SpawningDataEvent beforeSpawning => _beforeSpawning;

	[BoxGroup ( "PoolingParameters" ), SerializeField] private SpawningDataEvent _afterSpawning = null;
	public SpawningDataEvent afterSpawning => _afterSpawning;

	protected static readonly Vector3 origin = Vector3.zero;
	protected static readonly Quaternion identity = Quaternion.identity;

	protected virtual void Awake ( ) => this.AddThisInstance ( );
	protected virtual void OnDestroy ( ) => this.RemoveThisInstance ( );

	private void AddThisInstance ( )
	{
		if ( instances.Contains ( this ) ) return;
		this.dynamicIndex = instances.Count;
		instances.Add ( this );
		ReturnToPool ( );
	}

	private void RemoveThisInstance ( )
	{
		if ( instances.Contains ( this ) ) instances.Remove ( this );
		var currentAmountOfInstances = instances.Count;
		for ( int i = 0; i < currentAmountOfInstances; i++ )
			instances [ i ].dynamicIndex = i;
	}

	public static PooledBehaviour<T> Spawn
		( Object spawningObject, Vector3 position, Object customData = null, Transform parent = null, bool toggle = false )
		=> Spawn ( spawningObject, position, identity, customData, parent, toggle );

	public static PooledBehaviour<T> Spawn
		( Object spawningObject, Transform spawnPointReference, Object customData = null, Transform parent = null, bool toggle = false )
		=> Spawn ( spawningObject, spawnPointReference.position, spawnPointReference.rotation, customData, parent, toggle );

	public static PooledBehaviour<T> Spawn
		( Object spawningObject, Vector3 position, Quaternion rotation, System.Object customData = null, Transform parent = null, bool toggle = false )
	{
		if ( spawningObject == null )
		{
#if UNITY_EDITOR
			Debug.LogError (
			"You can't spawn an item from the pool without " +
			"passing the reference to who spawned It" );
#endif
			return null;
		}

		var newData = new SpawningData
		{
			spawningObject = spawningObject,
			position = position,
			rotation = rotation,
			customData = customData,
			parent = parent,
			toggle = toggle
		};

		return Spawn ( newData );
	}

	public static PooledBehaviour<T> Spawn ( SpawningData spawningData )
	{
		var instance = GetNextAvailableInstance ( );

		if ( instance == null )
		{
#if UNITY_EDITOR
			Debug.LogError (
			( spawningData.spawningObject as UnityEngine.Object ).name
			+ ": Is trying to spawn a "
			+ typeof ( T ).Name
			+ " from an insufficient amount of instances;" );
#endif
			return null;
		}

		if ( instance )
		{
			spawningData = instance.
				BeforeSpawning ( spawningData );

			instance._spawningData = spawningData;

			if ( spawningData.toggle )
				instance.gameObject.
					SetActive ( false );

			if ( instance.beforeSpawning.HasCalls )
				instance.beforeSpawning.
					Invoke ( instance, spawningData );

			instance.transform.
				SetPositionAndRotation ( spawningData.position, spawningData.rotation );

			if ( spawningData.parent != null )
				instance.transform.
					SetParent ( spawningData.parent );

			if ( spawningData.toggle )
				instance.gameObject.
					SetActive ( true );

			if ( instance.afterSpawning.HasCalls )
				instance.afterSpawning.
					Invoke ( instance, spawningData );

			spawningData = instance.
				AfterSpawning ( spawningData );

			instance._spawningData = spawningData;

			OnInstanceSpawn?.Invoke ( instance, spawningData);
		}
		return instance;
	}

	public static PooledBehaviour<T> GetNextAvailableInstance ( )
	{
		if ( !available )
			return null;

		var availableInstance = instances [ fence++ ];
		return availableInstance;
	}

	public virtual void ReturnToPool ( ) => ReturnToPool ( this );

	public static void ReturnToPool ( PooledBehaviour<T> instance )
	{
		if ( fence == 0 )
			return;
		//Entry Index
		var indexOfTheItemReturning = instance.dynamicIndex;

		//Saving before swaping the index of the items
		var instanceBackup = instances [ indexOfTheItemReturning ];

		//Swaping the items
		instances [ indexOfTheItemReturning ] = instances [ --fence ];
		instances [ fence ] = instanceBackup;

		//Updating the items cached index
		instances [ fence ].dynamicIndex = fence;
		instances [ indexOfTheItemReturning ].dynamicIndex = indexOfTheItemReturning;

		OnInstanceDespawn?.Invoke ( instance, instance._spawningData);
	}

	public delegate void PooledBehaviourDelegate(PooledBehaviour<T> instance, SpawningData spawnData );


	public static event PooledBehaviourDelegate OnInstanceSpawn;
	public static event PooledBehaviourDelegate OnInstanceDespawn;

	protected virtual SpawningData BeforeSpawning ( SpawningData spawnData )
		=> spawnData;

	protected virtual SpawningData AfterSpawning ( SpawningData spawnData )
		=> spawnData;
}