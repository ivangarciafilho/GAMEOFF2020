using Bloodstone;
using UnityEngine;

public class UmpauseParticles : MonoBehaviour
{
	public ParticleSystem[] particleSystems;
	[ReadOnly] public int amountOfParticles;

	public void Unpause ( )
	{
		for ( int i = 0; i < amountOfParticles; i++ )
			particleSystems [ i ].Play ( );
	}
#if UNITY_EDITOR
	private void OnValidate ( )
	{
		amountOfParticles = particleSystems.Length;
	}
#endif
}
