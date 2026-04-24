using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Enemy;

public class MovableObject : MonoBehaviour
{
    [SerializeField] private Afterburner _afterburnerModule;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _movementSpeed = 20.0f;
    [SerializeField] private float _turningSpeed = 2.5f;
    [SerializeField] private float _acceleration = 5.0f;
    // Debigging
    [SerializeField] private bool _autopilot = false;
    [SerializeField] private bool _verboseLogs = false;

    // These can be altered during afterburner use
    [SerializeField] private float currentSpeed = 0.0f;
    private float currentSpeedLimit = 20.0f;
    [SerializeField] private float currentAccel = 5.0f;
    private float turnAxisValue = 0.0f;
    //debugging
    private Vector3 _lastFiniteRotation;
    private Vector3 _lastStepRotation;

    public bool MovementEnabled { get; set; }

    // Events
    public UnityEvent OnAfterburnerOff;

    private void Start()
    {
        if (_afterburnerModule != null)
        {
            _afterburnerModule.OnAfterburnerOff.AddListener(()=>ToggleAfterburner(false));
        }
        currentAccel = _acceleration;
        currentSpeedLimit = _movementSpeed;
        MovementEnabled = true;
    }

    private void OnDisable()
    {
        MovementEnabled = false;
    }

    private void FixedUpdate()
    {
        if (MovementEnabled)
        {
            if(currentSpeed < currentSpeedLimit)
            {
                currentSpeed = Mathf.Clamp(currentSpeed + currentAccel * Time.fixedDeltaTime, 0, currentSpeedLimit);
            }
            else if(currentSpeed > currentSpeedLimit)
            {
                currentSpeed = Mathf.Clamp(currentSpeed - currentAccel * Time.fixedDeltaTime, currentSpeedLimit, currentSpeed);
            }
            
            UpdateLinearVelocity();
            if (turnAxisValue != 0f) // Player input turn only
                Turn(turnAxisValue > 0f);
            else if(_autopilot) //used in debugging
            {
                transform.rotation *= Quaternion.Euler(0, _turningSpeed / 2f * Time.fixedDeltaTime, 0);
            }
        }
    }

    public void OnTurn(InputAction.CallbackContext input)
    {
        turnAxisValue = input.ReadValue<float>();
    }

    // Turn towards object with _turningSpeed. Call in FixedUpdate!
    public void TurnToPoint(Vector3 worldPos)
    {
        if (_autopilot) return;//used in debugging

        var finiteForwardVec = Vector3.RotateTowards(transform.forward, worldPos - transform.position, Mathf.PI, 100f);
        float angleToPos = Vector3.SignedAngle(transform.forward, finiteForwardVec,  Vector3.up);
        float turnBy = Mathf.Clamp(angleToPos, -_turningSpeed * Time.fixedDeltaTime, _turningSpeed * Time.fixedDeltaTime);
        if(_verboseLogs)
            Debug.Log($"[{gameObject.name}] finiteForwardVec: {finiteForwardVec}, angleToPos: {angleToPos}, turnBy(step) {turnBy}");
        transform.rotation *= Quaternion.Euler(0, turnBy, 0);
        // Debugger info
        _lastFiniteRotation = finiteForwardVec.normalized;
        _lastStepRotation = transform.forward;
    }

    private void Turn(bool clockwise)
    {
        // Not using Rigidbody rotation because this works better around arena borders
        transform.rotation *= Quaternion.Euler(
            0, 
            (clockwise ? _turningSpeed : -_turningSpeed) * Time.fixedDeltaTime, 
            0);
    }

    private void UpdateLinearVelocity()
    {
        _rigidBody.velocity = _rigidBody.transform.forward * currentSpeed;
    }

    public bool CanUseAfterburner()
    {
        return _afterburnerModule != null && _afterburnerModule.CanUse();
    }    
    public void UseAfterburner()
    {
        if (_afterburnerModule == null)
            return;
        if (_afterburnerModule.TryUseAfterburner())
            ToggleAfterburner(true);
    }

    public void StopAfterburner()
    {
        _afterburnerModule?.StopAfterburner();
    }

    private void ToggleAfterburner(bool afterburnerOn)
    {
        currentSpeedLimit = afterburnerOn ? _movementSpeed + _afterburnerModule.SpeedBoost : _movementSpeed;
        currentAccel = afterburnerOn ? _acceleration + _afterburnerModule.AccelerationBoost : _acceleration;

        if(!afterburnerOn)
            OnAfterburnerOff.Invoke();
    }

    // renders current target when selected
   private void OnDrawGizmosSelected()
   {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.Scale(_lastStepRotation, new Vector3(3,0,3)));
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.Scale(_lastFiniteRotation, new Vector3(3, 0, 3)));
   }
}
