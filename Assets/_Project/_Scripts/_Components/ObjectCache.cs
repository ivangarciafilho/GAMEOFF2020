using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum InstanceType
{
	next,
	firstEnabled,
	firstDisabled
}

public class ObjectCache : MonoBehaviour
{
	public GameObject prefab;
	public int instancesToAdd = 0;
	[SerializeField] private GameObject [ ] instances;
	private int nextIndexToReturn = 0;
	[SerializeField] private bool timeSlicedInstantiation = true;
	[SerializeField] private string sceneToStoreInstances = null;
	private GameObject currentPrefabBeingInstantiated;


	private void Awake()
	{
		if ( instancesToAdd > 0 ) instances = new GameObject[instancesToAdd];
	}

	private IEnumerator Start ( )
	{ 
		if (instancesToAdd  > 0 && prefab !=null)
		{
			yield return null;

		//instances = new GameObject [ prefabCount ];

		var sendToAnotherScene = string.IsNullOrEmpty ( sceneToStoreInstances ) == false;
			var instantiationScene = gameObject.scene;

			for ( int i = 0; i < instancesToAdd; i++ )
			{
					currentPrefabBeingInstantiated = Instantiate ( prefab );
					currentPrefabBeingInstantiated.SetActive(false);

				if ( sendToAnotherScene ) 
				{
					while ( sendToAnotherScene && SceneLoaded( ) == false ) { yield return null; }

					bool SceneLoaded ( )
					{
						for ( int a = 0; a < SceneManager.sceneCount; ++a )
						{
							var scene = SceneManager.GetSceneAt ( a );
							if ( scene.name == sceneToStoreInstances ) return true; 
						}
						return false;
					}
				}

				if ( sendToAnotherScene )
				{
					instantiationScene = SceneManager.GetSceneAt ( FirstAvailableScene () );

					int FirstAvailableScene ()
					{
						for ( int b = 0; b < SceneManager.sceneCount; ++b )
						{
							var scene = SceneManager.GetSceneAt ( b );
							if ( scene.name == sceneToStoreInstances ) return b;
						}
						return 0;//yeah, by default, any mistake makes the instance goes right into the wrong scene
					}
				}

				SceneManager.MoveGameObjectToScene ( instances [ i ] , instantiationScene );
				if ( timeSlicedInstantiation ) yield return null;
			}

		if ( sendToAnotherScene && gameObject.scene != instantiationScene) 
			SceneManager.MoveGameObjectToScene ( gameObject, instantiationScene );
		}
	}

	//First Enabled Instance
	public GameObject SpawnFirstEnabledInstance ( 
		Vector3 position, 
		Quaternion rotation, 
		Space coordinatesType = Space.Self, 
		Transform parentTo = null )
		=> SpawnFirstInstance ( 
			InstanceType.firstEnabled, 
			position, 
			rotation, 
			coordinatesType, 
			parentTo ); 

	//First Disabled Instannce
	public GameObject SpawnFirstDisabledInstance ( 
		Vector3 position, 
		Quaternion rotation, 
		Space coordinatesType = Space.Self, 
		Transform parentTo = null )
		=> SpawnFirstInstance( 
			InstanceType.firstDisabled,  
			position, 
			rotation, 
			coordinatesType, 
			parentTo);
		
	public GameObject SpawnFirstInstance (
		InstanceType instannceType, 
		Vector3 position, 
		Quaternion rotation, 
		Space coordinatesType, 
		Transform parentTo = null )
	{
		GameObject objectToReturn = null;

		switch ( instannceType )
		{
			case InstanceType.next:
				objectToReturn = GetNextInstance ( );
				break;
			case InstanceType.firstEnabled:
				objectToReturn = GetFirstEnabledInstance ( );
				break;
			case InstanceType.firstDisabled:
				objectToReturn = GetFirstDisabledInstance ( );
				break;
			default:
				break;
		}

		if ( objectToReturn != null ) 
			UpdateInstanceTransformations ( 
				objectToReturn.transform, 
				coordinatesType, 
				position, 
				rotation, 
				parentTo );

		return objectToReturn;
	}

	public GameObject GetNextInstance ( bool toggleOn = false)
	{
		if ( nextIndexToReturn >= instances.Length ) nextIndexToReturn = 0;

		var availableInstance = instances [ nextIndexToReturn ];
		if ( toggleOn ) availableInstance.SetActive ( false );

		nextIndexToReturn++;
		return availableInstance;
	}

	public GameObject GetFirstEnabledInstance ( ) 
		=> instances.FirstOrDefault(_instance => _instance.activeSelf );

	public GameObject GetFirstDisabledInstance ( )
		=> instances.FirstOrDefault ( _instance => _instance?.activeSelf == false );

	private void UpdateInstanceTransformations ( 
		Transform instanceTransform,
		Space coordinatesType, 
		Vector3 position,
		Quaternion rotation,
		Transform parentTo = null )
	{
		if ( parentTo ) instanceTransform.SetParent ( parentTo );

		switch ( coordinatesType )
		{
			case Space.World:
				{
					instanceTransform.position = position;
					instanceTransform.rotation = rotation;
					break;
				}
			case Space.Self:
				{
					instanceTransform.localPosition = position;
					instanceTransform.localRotation = rotation;
					break;
				}

			default:
				throw new System.Exception ( "Unexpected Case" );
		}
	}
}