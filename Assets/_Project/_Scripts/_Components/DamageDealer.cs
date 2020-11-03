using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
	public LayerMask maskFilter;
	public OnDamageDealtEvent OnDamageDealt = new OnDamageDealtEvent();
	public float damage = 20.0f;

	private void OnTriggerEnter(Collider collision)
	{
		InflictDamageOn(collision);
	}

	public void InflictDamageOn(Collider collision)
	{
		if (collision)
		{

			var obj = collision.gameObject;

			if (GameUtils.LayerMaskContains(obj.layer, maskFilter))
			{
				if (Health.HealthByObject.ContainsKey(obj))
				{
#if UNITY_EDITOR
					Debug.Log("Damage! " + collision.gameObject.name);
#endif

					var h = Health.HealthByObject[obj];
					h.Change(-damage);
				}

				OnDamageDealt.Invoke(damage);
			}
		}
	}
}
