using System;
using UnityEngine;

public class KnifeBlade : MonoBehaviour
{
    [HideInInspector]
    public Action OnKnifeBladeCollided;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("SOU A FACA E COLIDIIII");
        //OnKnifeBladeCollided?.Invoke();
    }
}
