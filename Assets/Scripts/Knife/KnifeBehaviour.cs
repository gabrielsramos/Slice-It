using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class KnifeBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private InputAction _inputAction;
    [SerializeField] private KnifeBlade _blade;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private float _powerUpTime = 2f;
    [Header("Impulse parameters")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Vector3 _velocityDirection = new Vector3(0, 1, 0);
    [Header("Direction which should slowdown parameters")]
    [SerializeField] private Vector3 _cutDirection = new Vector3(1, 0, 0);
    [Range(10f, 90f)]
    [SerializeField] private float _cutDegreesThreshold = 60f;
    [SerializeField] private float _rotationVelocityFactor = 100f;
    [SerializeField] private float _rotationNormalMultiplier = 1f;
    [SerializeField] private float _rotationFasterMultiplier = 3f;
    [SerializeField] private float _rotationPowerUpMultiplier = 8f;
    [Header("Bottom floor")]
    [SerializeField] private Transform _floor;

    [HideInInspector]
    public Action OnKnifeFall;
    [HideInInspector]
    public Action<int> OnHitBlock;
    [HideInInspector]
    public Action OnHitFinishWall;

    private Rigidbody _rigidBody;
    private Quaternion _initialRotation;
    private bool _isGrounded;
    private bool _isInFirstSpin;
    private bool _powerUpActivated;
    private Coroutine _rotateCoroutine;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _initialRotation = _rigidBody.rotation;
    }

    private void OnEnable()
    {
        _inputAction.Enable();
        _inputAction.performed += ClickAction;
        _blade.OnKnifeBladeCollided += BladeCollided;
    }

    private void OnDisable()
    {
        _inputAction.Disable();
        _inputAction.performed -= ClickAction;
        _blade.OnKnifeBladeCollided -= BladeCollided;
    }

    private void BladeCollided()
    {
        OnHitFinishWall?.Invoke();
    }

    private void ClickAction(InputAction.CallbackContext context)
    {
        if (!_isInFirstSpin)
        {
            ApplyForceToKnife();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            _isGrounded = true;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.MoveRotation(_initialRotation);
        }
    }

    private void OnDrawGizmos()
    {
        //Draw the direction which the velocity vector is pointing to
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _velocityDirection);
        Gizmos.DrawSphere(transform.position + _velocityDirection, 0.01f);

        //Draw the direction which the cut vector is pointing to
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, transform.position);
        Gizmos.DrawLine(transform.position, transform.position + _cutDirection);
        Gizmos.DrawSphere(transform.position + _cutDirection, 0.05f);

#if UNITY_EDITOR
        //Draws an arc to display the area in which the knife rotation is going to slow down
        Handles.color = IsInCutDirection() ? Color.green : Color.red;
        Vector3 arcDirection = _cutDirection;
        Handles.DrawSolidArc(transform.position, transform.right, arcDirection, _cutDegreesThreshold, 1f);
        Handles.DrawSolidArc(transform.position, transform.right, arcDirection, -_cutDegreesThreshold, 1f);
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(transform.position, transform.position + transform.forward);
#endif
    }

    private void FixedUpdate()
    {
        CheckIfFallToFloor();
    }

    private void LateUpdate()
    {
        if (IsInCutDirection())
        {
            _trail.emitting = true;
        }
        else
        {
            _trail.emitting = false;
        }
    }

    private void CheckIfFallToFloor()
    {
        if (transform.position.y <= _floor.position.y)
        {
            OnKnifeFall?.Invoke();
        }
    }

    private bool IsInCutDirection()
    {
        Vector3 facingDirection = transform.forward;
        Vector3 referenceDirection = _cutDirection;
        float angleBetween = Vector3.Angle(referenceDirection, facingDirection);
        return angleBetween < _cutDegreesThreshold;
    }

    private void ApplyForceToKnife()
    {
        if (_rotateCoroutine != null )
        {
            StopCoroutine(_rotateCoroutine);
        }
        _rotateCoroutine = StartCoroutine(RotateUntilGrounded());
    }

    private IEnumerator RotateUntilGrounded()
    {
        Quaternion rotation;
        Vector3 axisRotation = transform.right;
        _isGrounded = false;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.velocity = _velocityDirection.normalized * _speed;

        yield return new WaitForEndOfFrame();

        if (_powerUpActivated)
        {
            yield break;
        }

        _isInFirstSpin = true;

        while (IsInCutDirection())
        {
            rotation = Quaternion.Euler(_rotationVelocityFactor * _rotationFasterMultiplier * Time.deltaTime * axisRotation);
            _rigidBody.MoveRotation(rotation * _rigidBody.rotation);
            yield return null;
        }
        _isInFirstSpin = false;

        while (!_isGrounded)
        {
            float rotationFactor = IsInCutDirection() ? _rotationVelocityFactor * _rotationNormalMultiplier : _rotationVelocityFactor * _rotationFasterMultiplier;
            rotation = Quaternion.Euler(rotationFactor * Time.deltaTime * axisRotation);
            _rigidBody.MoveRotation(rotation * _rigidBody.rotation);
            yield return null;
        }
    }

    private IEnumerator PowerUpRotate()
    {
        float timeLeft = _powerUpTime;
        Vector3 axisRotation = transform.right;

        while (timeLeft >= 0.0f)
        {
            timeLeft -= Time.deltaTime;

            var rotation = Quaternion.Euler(_rotationVelocityFactor * _rotationPowerUpMultiplier * Time.deltaTime * axisRotation);
            _rigidBody.MoveRotation(rotation * _rigidBody.rotation);

            yield return null;
        }
        _powerUpActivated = false;
    }

    public void CountBlockPoints(int points)
    {
        _audioSource.Play();
        OnHitBlock?.Invoke(points);
    }

    public void ActivatePowerUp()
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
        }
        _powerUpActivated = true;
        StartCoroutine(PowerUpRotate());
    }
}
