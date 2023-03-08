using System;
using UnityEngine;

public class Slice : MonoBehaviour
{
    [NonSerialized]
    public Action<KnifeBehaviour> OnCollisionWithKnife;

    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Knife"))
        {
            var knife = other.gameObject.transform.parent.GetComponentInParent<KnifeBehaviour>();
            OnCollisionWithKnife?.Invoke(knife);
        }
    }

    public void ThrowPieceOut(Vector3 force)
    {
        _rigidBody.AddForce(force, ForceMode.Impulse);
    }

}