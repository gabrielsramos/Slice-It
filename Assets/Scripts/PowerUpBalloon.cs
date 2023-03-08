using System;
using UnityEngine;

public class PowerUpBalloon : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [HideInInspector]
    public Action OnKnifeBladeCollided;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Knife"))
        {
            _audioSource.Play();
            OnKnifeBladeCollided?.Invoke();
            Destroy(gameObject);
        }
    }
}
