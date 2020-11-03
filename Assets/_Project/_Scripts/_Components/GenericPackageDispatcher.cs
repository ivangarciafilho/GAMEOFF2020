using System.Collections;
using System.Collections.Generic;
using Bloodstone;
using UnityEngine;

public class GenericPackageDispatcher : MonoBehaviour
{
	[SerializeField] private string _key;
	public string key => _key;

	[ReadOnly,SerializeField] private int _keyhash;
	public int keyhash => _keyhash;

	[SerializeField] private DataPackage _currentPackage;
	public DataPackage currentPackage => _currentPackage;

	public void Dispatch ( )
		=> GenericPackageReceiver.ReceiveDispatch ( this );

	public void Dispatch ( string key, DataPackage package )
	{
		SetDispatcher ( key, package );
		GenericPackageReceiver.ReceiveDispatch ( this );
	}

	public void Dispatch ( string key)
	{
		SetKey(key);
		GenericPackageReceiver.ReceiveDispatch ( this );
	}

	public void Dispatch ( DataPackage package)
	{
		SetPackage ( package );
		GenericPackageReceiver.ReceiveDispatch ( this );
	}

	public void SetKey ( string key)
		=>_keyhash = ((_key = key)).ToHash();

	public void SetPackage ( DataPackage package )
		=> _currentPackage = package;

	public void SetDispatcher ( string key, DataPackage package )
	{
		SetKey ( key );
		SetPackage ( package );
	}


	public void DispatchFromAnimationEvent ( string key )
		=> Dispatch ( key );

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		_keyhash = _key.ToHash();
	}
#endif
}
