using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FireEventOnParticleCollision : MonoBehaviour
{
    public ParticleSystem itsParticleSytem;
    private ParticleCollisionEvent [ ] collisionEvents = new ParticleCollisionEvent[3000];
    public bool callEventsJustOnce = true;
    public OnParticleCollisionEvent triggeredEvents;
    public OnParticleCollisionColliderOnlyEvent triggeredColliderOnlyEvents;

    bool calledEvents = false;

    private void OnEnable()
    {
        calledEvents = false;
    }

    private void OnParticleCollision ( GameObject other )
    {
        if (callEventsJustOnce && calledEvents) return;

        itsParticleSytem.GetCollisionEvents ( other, collisionEvents );

        //for (int i = 0; i < collisionEvents.Length; i++)
        {
            if(collisionEvents[0].colliderComponent)
                triggeredColliderOnlyEvents.Invoke((Collider)collisionEvents[0].colliderComponent);
        }
        
        triggeredEvents.Invoke ( itsParticleSytem, other, collisionEvents );

        calledEvents = true;
    }

#if UNITY_EDITOR
    private void OnValidate ( )
    {
        if ( itsParticleSytem == null ) itsParticleSytem = GetComponent<ParticleSystem>();
    }
#endif
}
