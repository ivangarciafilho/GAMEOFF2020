using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleSystemStateCache : MonoBehaviour
{
	[SerializeField] private ParticleSystem[] _nestedParticleSystems;
	public ParticleSystem[] nestedParticleSystems => _nestedParticleSystems;

	[SerializeField] private int _amountOfnestedParticleSystems;
	public int amountOfnestedParticleSystems => _amountOfnestedParticleSystems;

	public int runningParticleSystems => nestedParticleSystems.Where (_item =>  (_item.isPlaying || _item.isEmitting )).Count();
	public int idleParticleSystems => nestedParticleSystems.Where ( _item => ( _item.isPlaying == false && _item.isEmitting == false ) ).Count ( );

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		_nestedParticleSystems = GetComponentsInChildren<ParticleSystem> ( ).ToArray();
		_amountOfnestedParticleSystems = _nestedParticleSystems.Length;
	}
#endif
}
