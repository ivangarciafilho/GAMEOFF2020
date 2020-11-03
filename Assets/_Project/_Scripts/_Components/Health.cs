using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Health : MonoBehaviour
{
	public static Dictionary<GameObject, Health> HealthByObject 
		=  new Dictionary<GameObject, Health>();
	

	[SerializeField] private float _maxHealth = 100.0f;
	public float maxHealth => _maxHealth;

	[SerializeField] private float currentHealth;
	public float currentHealthNormalized 
		=> Mathf.Clamp01(currentHealth / maxHealth);

	public float Value { get; private set; }

	public FloatEvent OnValueIsChanged;
	public FloatEvent  OnReachesZero;
	public FloatEvent OnAlive;
	public FloatEvent passDamageTaken;
	public FloatEvent passRemainingHealth;
	public FloatEvent passRemainingHealthNormalized_01;
	public FloatEvent passRemainingHealthNormalized_10;

	private void Awake() => Value = maxHealth;
	private void OnEnable() => HealthByObject.Add(gameObject, this);
	private void OnDisable()  => HealthByObject.Remove(gameObject);
	private void LateUpdate() => currentHealth = Value;
	public void ResetHealth() => Value = maxHealth;

	public void BruteForceChangeTheHealth_DontUseThisPublicly(float value)
	{
		var oldValue = Value;

		Value = value;

		if(Value > oldValue && oldValue <= 0)
		{
			OnAlive.Invoke(Value);
		}
		else if(Value <= 0.0f && oldValue > 0)
		{
			OnReachesZero.Invoke(Value);
		}
	}

	public void Change(float value)
	{
		if (Value <= 0.0f && (Value + value) <= 0) return;

		Value += value;
		OnValueIsChanged.Invoke(Value);

		if (Value - value <= 0.0f && (Value + value) > 0)
			OnAlive.Invoke(Value);

		if (Value <= 0.0f)
		{
			Value = 0.0f;
			OnReachesZero.Invoke(Value);
		}

		if ( value < 0 )
		{
			if ( passDamageTaken.HasCalls ) 
				passDamageTaken.Invoke ( value);
		}

		passRemainingHealth.
			Invoke (value);

		passRemainingHealthNormalized_01.
			Invoke ( currentHealthNormalized );

		passRemainingHealthNormalized_10.
			Invoke ( 1 - currentHealthNormalized );
	}
}
