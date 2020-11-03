using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloodstone;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;
using ReadOnly = Bloodstone.ReadOnlyAttribute;

[ExecuteInEditMode, ExecuteAlways]
public class FixedUpdateDispatcher : MonoBehaviour, IConstantDispatcher
{
	public Timeframe schedule => Timeframe.onFixedUpdate;

	[ReorderableList, SerializeField] private MonoBehaviour [ ] _receiverBehaviours = new MonoBehaviour[0];

	private IConstantReceiver [ ] _receivers = new IConstantReceiver[0];

	public IConstantReceiver [ ] receivers => _receivers;

	[ReadOnly] public int currentAmountOfReceivers;
	[ReadOnly, SerializeField] public float timeScale;
	[ReadOnly] public float time;
	[ReadOnly] public float fixedDeltatime;

	private void FixedUpdate ( )
	{
		timeScale = Time.timeScale;
		time = Time.time ;
		fixedDeltatime = Time.fixedDeltaTime;
		currentAmountOfReceivers = _receivers.Length;

		for ( int i = 0; i < currentAmountOfReceivers; i++ )
			_receivers [ i ].OnSignal ( Timeframe.onFixedUpdate, timeScale,time, fixedDeltatime);
	}

#if UNITY_EDITOR
	private void Update ( )
		=> CacheReceivers ( );

	private void OnValidate ( )
		=> CacheReceivers ( );

	void CacheReceivers ( )
	{
		if ( _receiverBehaviours == null )
			_receiverBehaviours = new MonoBehaviour [ 0 ];

		_receiverBehaviours.
			Where ( _item => _item != null ).
			ToArray ( ) ;

		var validMonobehaviours = new List<MonoBehaviour>();
		var validReceivers = new List<IConstantReceiver>();
		var behavioursCount = _receiverBehaviours.Length;

		for ( int i = 0; i < behavioursCount; i++ )
			if ( _receiverBehaviours [ i ] is IConstantReceiver )
			{
				validMonobehaviours.Add ( _receiverBehaviours [i]);
				validReceivers.Add ( _receiverBehaviours [i] as IConstantReceiver );
			}

		_receiverBehaviours = validMonobehaviours.ToArray ( );

		if ( _receiverBehaviours.Length < 1 )
			_receiverBehaviours = GetComponentsInChildren<MonoBehaviour> ( );

		_receiverBehaviours.
			Where ( _item => _item is IConstantReceiver).
			ToArray ( ) ;

		if ( _receivers == null )
			_receivers = new IConstantReceiver [ 0 ];

		if ( _receiverBehaviours.Length > 0 )
		{
			_receivers = validReceivers.ToArray();
			currentAmountOfReceivers = _receivers.Length;
		}
	}
#endif
}
