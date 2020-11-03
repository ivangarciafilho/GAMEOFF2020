using System;
using UnityEngine;
using UltEvents;

[Serializable] public class TriggerEvent : UltEvent<ColliderEventType, Collider, PairSpacialRelationship> { }
