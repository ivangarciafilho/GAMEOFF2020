using UnityEngine;

public class SetToLoopOnRigidbodyMotion : MonoBehaviour
{
	public Rigidbody2D triggeringRigidbody;
	public ParticleSystem itsParticleSystem;
	public ParticleSystem.MainModule mainModule;

	private void OnEnable ( ) => mainModule = itsParticleSystem.main;


	private void FixedUpdate ( )
	{
		mainModule.loop = triggeringRigidbody.velocity.magnitude > 0;

		if (		triggeringRigidbody.velocity.magnitude > 0
			&&	itsParticleSystem.isEmitting == false
			)
		{
			itsParticleSystem.Play ( );
		}
	}
}
