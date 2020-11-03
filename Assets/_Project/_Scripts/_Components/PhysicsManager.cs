using System.Linq;
using Bloodstone;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody = null;
    public Rigidbody itsRigidbody => _rigidbody;

    [SerializeField] private Collider[] _colliders = null;
    public Collider[] colliders => _colliders.ToArray();

    [SerializeField, ReadOnly] private int _amountOfColliders = 0;
    public int amountOfColliders => _amountOfColliders;

    [SerializeField] private CollisionDetectionMode _collisionDetectionWithoutPhysics 
        = CollisionDetectionMode.ContinuousSpeculative;
    public CollisionDetectionMode collisionDetectionWithoutPhysics 
        => _collisionDetectionWithoutPhysics;


    [SerializeField] private CollisionDetectionMode _collisionDetectionWithPhysics
        = CollisionDetectionMode.Discrete;
    public CollisionDetectionMode collisionDetectionWithPhysics 
        => _collisionDetectionWithPhysics;

    private static readonly Vector3 zeroVelocity = new Vector3(0f, 0f, 0f);
    public void RemovePhysics()
    {
        StopRigidbody();
        _rigidbody.detectCollisions = false;
        _rigidbody.collisionDetectionMode = _collisionDetectionWithoutPhysics;
        _rigidbody.isKinematic = true;

        colliders.SetEnabled();
        _rigidbody.Sleep ( );
    }

    public void ReturnPhysics()
    {
        StopRigidbody();
        _rigidbody.isKinematic = false;
        _rigidbody.collisionDetectionMode = _collisionDetectionWithPhysics;

        colliders.SetEnabled(false);
        _rigidbody.detectCollisions = true;
        _rigidbody.WakeUp ( );
    }

    public void StopRigidbody()
    {
        _rigidbody.velocity = zeroVelocity;
        _rigidbody.angularVelocity = zeroVelocity;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null) _rigidbody = GetComponentsInChildren<Rigidbody>()[0];
        if (_colliders == null || _colliders.Length < 1) _colliders = GetComponentsInChildren<Collider>();
        _colliders = _colliders.Where(_item => _item != null).ToArray();
        _amountOfColliders = _colliders.Length;
    }
#endif
}