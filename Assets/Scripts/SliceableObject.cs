using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    [Header("Design variables")]
    [SerializeField] private int _pointsToReward;
    [SerializeField] private float _impactForce;

    [Header("Others")]
    [SerializeField] private Slice _leftSlice;
    [SerializeField] private Slice _rightSlice;

    private bool _hasCollidedAlready;

    private void OnEnable()
    {
        _leftSlice.OnCollisionWithKnife += OnKnifeCollision;
        _rightSlice.OnCollisionWithKnife += OnKnifeCollision;
    }

    private void OnDisable()
    {
        _leftSlice.OnCollisionWithKnife -= OnKnifeCollision;
        _rightSlice.OnCollisionWithKnife -= OnKnifeCollision;
    }

    private void OnKnifeCollision(KnifeBehaviour knife)
    {
        if (_hasCollidedAlready)
        {
            return;
        }

        knife.CountBlockPoints(_pointsToReward);
        _hasCollidedAlready = true;
        Vector3 leftNormal = transform.forward * _impactForce;
        _leftSlice.ThrowPieceOut(leftNormal);

        Vector3 rightNormal = -transform.forward * _impactForce;
        _rightSlice.ThrowPieceOut(rightNormal);
    }
}
