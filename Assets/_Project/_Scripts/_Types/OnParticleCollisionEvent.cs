using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable] public class OnParticleCollisionEvent : UnityEvent<ParticleSystem, GameObject, ParticleCollisionEvent [ ]> { }
