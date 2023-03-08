using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishWall : MonoBehaviour
{
    [HideInInspector]
    public Action OnKnifeBladeCollided;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Knife"))
        {
            OnKnifeBladeCollided?.Invoke();
        }
    }
}
