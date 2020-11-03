using System.Collections.Generic;
using Bloodstone;
using UnityEngine;
using System.Linq;

public struct DamadeableDefinition
{
	public delegate void OnCall(Damageable d);
	public OnCall onCall;

	public DamadeableDefinition(OnCall _onCall)
	{
		this.onCall = _onCall;
	}
}

public class Damageable : MonoBehaviour
{
	private static Dictionary<int, Damageable> instances = new Dictionary<int, Damageable>();

	[SerializeField] private string evt;
	public string Evnt => evt;

	[SerializeField] private float _maximumHealth;
	public float maximumHealth => _maximumHealth;

	[ReadOnly, SerializeField] private float _damageTaken;
	public float damageTaken => _damageTaken;

	public float remainingHealth => _maximumHealth - _damageTaken;

	public float remainingHealthNormalized => remainingHealth / maximumHealth;

	private void Awake()
		=> instances.AddIfNotExists(gameObject.GetInstanceID(), this);

	public delegate float DamageableDelegate ( Damageable damageable);

	public static void UpdateHealth ( int instanceKey, DamageableDelegate updateOperation)
	{
		if ( instances.ContainsKey ( instanceKey ) == false ) return;
		var instance = instances [ instanceKey ];

		var resultingIncrement = (float) ((updateOperation == null) 
			?  0f : updateOperation?.Invoke( instance ));

		if ( resultingIncrement != 0 )
			instance.UpdateHealth ( resultingIncrement );
	}

	public delegate float HealthUpdateDelegate (Damageable damageable, float value);
	public static event HealthUpdateDelegate onHealthUpdate;
	public static event HealthUpdateDelegate onHealthDecreased;
	public static event HealthUpdateDelegate onHealthIncreased;

	private void UpdateHealth ( float amount)
	{
		onHealthUpdate?.Invoke ( this, amount);

		if ( amount < 0 )
		{
			DecreaseHealth ( amount );
		}
		else
		{
			IncreaseHealth ( amount );
		}
	}

	private void IncreaseHealth ( float amount)
	{
		_damageTaken -= Mathf.Abs (amount);
		_damageTaken = Mathf.Clamp (_damageTaken , 0, _maximumHealth);

		onHealthIncreased?.Invoke ( this, amount );
	}

	private void DecreaseHealth ( float amount)
	{
		amount = Mathf.Abs ( amount );

		_damageTaken += amount;
		_damageTaken = Mathf.Clamp (_damageTaken , 0, _maximumHealth);

		onHealthDecreased?.Invoke(this,amount);
	}

	public static void SendEvent(GameObject go, DamadeableDefinition dd, string evt)
	{
		var result = instances.FirstOrDefault(it => it.Key == go.GetInstanceID());
		if (result.Key != null)
		{
			if(result.Value.Evnt == evt)
			{
				dd.onCall(result.Value);
			}
		}
	}
}
