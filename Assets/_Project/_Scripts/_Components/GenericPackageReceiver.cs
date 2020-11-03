using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bloodstone;

public class GenericPackageReceiver : MonoBehaviour
{
	[SerializeField] private string _key;
	public string key => _key;

	[ReadOnly,SerializeField] private int _keyhash;
	public int keyhash => _keyhash;

	[ReadOnly,SerializeField] private DataPackage _currentPackage;
	public DataPackage currentPackage => _currentPackage;

	private static Dictionary<int, List<GenericPackageReceiver>> receivers 
		= new Dictionary<int, List<GenericPackageReceiver>> ( );

	public Vector3Event passDispatcherPosition;
	public DataPackageEvent onMessageReceived;

	public void Awake ( )
	{
		if ( receivers.ContainsKey( _keyhash )  == false)
			receivers.Add ( _keyhash, new List<GenericPackageReceiver> ( ) );

		receivers [ _keyhash ].AddIfNotContains ( this );
	}

    private void OnDestroy()
    {
		receivers.Clear();
	}

    public static void ReceiveDispatch ( GenericPackageDispatcher dispatcher )
	{
		if ( receivers.ContainsKey ( dispatcher.keyhash ) == false ) return;

		var availableReceivers = receivers [ dispatcher.keyhash ];
		var amountOfReceivers = availableReceivers.Count;

		for ( int i = 0; i < amountOfReceivers ; i++ )
			availableReceivers[ i ].SendPackage ( dispatcher );
	}

	private void SendPackage ( GenericPackageDispatcher dispatcher)
	{
		_currentPackage = dispatcher.currentPackage;

		if ( passDispatcherPosition.HasCalls ) 
			passDispatcherPosition.Invoke ( dispatcher.transform.position );

		if ( onMessageReceived.HasCalls ) 
			onMessageReceived.Invoke ( _currentPackage );
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		_keyhash = _key.ToHash();
	}
#endif
}
