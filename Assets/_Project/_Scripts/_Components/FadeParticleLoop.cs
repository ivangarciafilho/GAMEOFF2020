using System.Linq;

using UnityEngine;

public class FadeParticleLoop : MonoBehaviour
{
	public ParticleSystem[] particleSystems;
	[SerializeField] private int amountOfParticles;
	[SerializeField] private ParticleSystem.MainModule[] mainModules;

    private void Start()
    {
		GetAll();
    }

    public void FadeIn ( )
	{
		for ( int i = 0; i < amountOfParticles; i++ )
		{
			mainModules [ i ].loop = true;

			if ( particleSystems [ i ].isPaused
				|| particleSystems[ i ].isStopped
				|| particleSystems[ i ].isEmitting == false
				|| particleSystems[ i ].isPlaying == false )
			{
				particleSystems[i].Play ( );
			}
		}
	}

	public void Fadeout ( )
	{
		for ( int i = 0; i < amountOfParticles; i++ )
			mainModules [ i ].loop = false;
	}

#if UNITY_EDITOR
	private void OnValidate ( )
	{
		GetAll();
	}
#endif

	void GetAll()
    {
		if (particleSystems == null)
			particleSystems = new ParticleSystem[0];

		particleSystems = particleSystems.
			Where(_item => _item != null).
			ToArray();

		particleSystems = particleSystems.
			Distinct().
			ToArray();

		if (particleSystems.Length == 0)
			particleSystems = GetComponentsInChildren<ParticleSystem>();

		particleSystems = particleSystems.
			Where(_item => _item.main.loop == true).
			ToArray();

		amountOfParticles = particleSystems.Length;

		mainModules = new ParticleSystem.
			MainModule[amountOfParticles];

		for (int i = 0; i < amountOfParticles; i++)
			mainModules[i] = particleSystems[i].main;
	}

}
