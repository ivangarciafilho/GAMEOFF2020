using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent (typeof (ParticleSystem))]
public class PauseAfterDelay : MonoBehaviour
{
    [SerializeField] private ParticleSystem itsParticleSystem;
    [SerializeField] private float delay = 2f;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(WaitAndPause());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator WaitAndPause()
    {
        yield return new WaitForSeconds(delay);
        itsParticleSystem.Pause();
    }
}
