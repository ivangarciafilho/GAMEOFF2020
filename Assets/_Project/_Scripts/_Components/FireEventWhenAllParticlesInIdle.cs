using UnityEngine;
using UnityEngine.Events;

public class FireEventWhenAllParticlesInIdle : MonoBehaviour
{
    public UnityEvent triggeredEvents;
    public ParticleSystemStateCache stateCache;

    private void Update ( )
    {
        if(stateCache != null)
            if ( stateCache.runningParticleSystems < 1 ) 
                triggeredEvents.Invoke ( );
    }

#if UNITY_EDITOR
    private void OnValidate ( )
    {
        if ( stateCache == null ) stateCache = GetComponent<ParticleSystemStateCache>();
    }
#endif
}
