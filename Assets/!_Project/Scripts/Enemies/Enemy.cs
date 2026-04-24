using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyController EnemyController; //do we need it?
    public enum EnemyBehavior
    {
        None,
        CloseQuatersAttack,
        AttackFromDistance,
        Circling,
        Retreating
    }

    [Range(1.00f, 100f)]
    [SerializeField] private float _missileAttackDistance = 30.0f;
    [Range(1.00f, 100f)]
    [SerializeField] private float _ramAttackDistance = 15.0f;
    [Range(1.00f, 100f)]
    [SerializeField] private float _attackRepeatDelay = 10.0f;
    [Range(1.00f, 100f)]
    [SerializeField] private float _playerTargetingOvershootMax = 2.0f;
    [Range(0.02f, 2f)]
    [SerializeField] private float _distanceLossToAbortRamming = 0.2f;

    [SerializeField] private DamageTaker _hpModule;
    [SerializeField] private MovableObject _movementModule;
    [SerializeField] private EnemyWeapons _weaponsModule;
    [SerializeField] private Rigidbody _rigidBody;

    private float _attackRepeatCd = 0f;
    private float _lastDistanceToPlayer = 0f;
    private Vector3 _currentTarget = new Vector3(0,0,0);
    private EnemyBehavior _currentBehavior = EnemyBehavior.None;

    public float PlayerTargetingOvershootMax { get => _playerTargetingOvershootMax; }
    public float MissileAttackDistance { get => _missileAttackDistance; }
    // Updates target for the Enemy
    public Vector3 CurrentTarget 
    {
        get => _currentTarget; 
        set
        {
            _currentTarget = value;
            _weaponsModule.CurrentTarget = _currentTarget;
        }
    }
    // Sets Enemy behavior
    public EnemyBehavior CurrentBehavior 
    { 
        get => _currentBehavior;  
        set
        {
            if(_currentBehavior == value) return;
            _currentBehavior = value;
            OnBehaviorUpdated();
        }
    }
    public bool IsDead { get => _hpModule.IsDead(); }


    private void Start()
    {
        _hpModule.OnDeath.AddListener(
            ()=>
            {
                if(_rigidBody != null)
                {
                    _rigidBody.detectCollisions = false;
                    _rigidBody.freezeRotation = true;
                }
            });

        // Stops ramming attack on Afterburer turning off
        _movementModule?.OnAfterburnerOff.AddListener(() =>
            {
                _weaponsModule?.toggleRam(false);
                _attackRepeatCd = _attackRepeatDelay;
                CurrentBehavior = EnemyBehavior.Retreating; // retreat after the attack
            });
    }

    private void Update()
    {
        if (_hpModule.IsDead())
            return;
        if (_attackRepeatCd > 0f)
        {
            _attackRepeatCd -= Time.deltaTime;
            if(_attackRepeatCd <= 0f)
            {
                _attackRepeatCd = 0f;
                CurrentBehavior = EnemyBehavior.None; // await new orders from EnemyController
            }
        }
    }

    private void FixedUpdate()
    {
        if (_hpModule.IsDead())
            return;

        switch(_currentBehavior)
        {
            case EnemyBehavior.None:
                break;

            // Waiting for its turn to attack
            case EnemyBehavior.Circling:
                _movementModule?.TurnToPoint(_currentTarget);
                break;

            case EnemyBehavior.AttackFromDistance:
                _movementModule?.TurnToPoint(_currentTarget);
                Vector3 toTargetVector = _currentTarget - transform.position;
                float distToTarget = toTargetVector.magnitude;
                if (distToTarget <= _missileAttackDistance // enemy is within firing distance
                    && Vector3.Dot(transform.forward, toTargetVector.normalized) > 0.6) // enemy is turned towards the player
                    FireMissile();
                break;

            case EnemyBehavior.CloseQuatersAttack:
                _movementModule?.TurnToPoint(_currentTarget);
                Vector3 toRamTargetVector = _currentTarget - transform.position;
                float distToRamTarget = toRamTargetVector.magnitude;
                if (
                    distToRamTarget <= _ramAttackDistance
                    && !_weaponsModule.IsRamming
                    && _movementModule.CanUseAfterburner()
                    && Vector3.Dot(transform.forward, toRamTargetVector.normalized) > 0.75 // enemy is turned towards the player
                    )
                {
                    RammingAttack();
                }
                else if(_weaponsModule.IsRamming)
                {
                    // If increasing distance to player - abort ramming
                    if((_lastDistanceToPlayer - distToRamTarget) < _distanceLossToAbortRamming)
                    {
                        //Debug.Log($"[{gameObject.name}] Distance to player increased by {(_lastDistanceToPlayer - distToRamTarget)} - aboring Ramming attack!");
                        _movementModule?.StopAfterburner();
                    }
                }
                _lastDistanceToPlayer = distToRamTarget;
                break;

            // Try to fly away from player
            case EnemyBehavior.Retreating:
                _movementModule?.TurnToPoint(_currentTarget);
                break;
        }
    }

    private void OnBehaviorUpdated()
    {
        if(_currentBehavior == EnemyBehavior.Retreating)
        {
            Vector3 forwardVec = transform.forward;
            float retreatVectorLength = 50f;
            Vector3 direction0 = new Vector3( forwardVec.z * retreatVectorLength, 0, -forwardVec.x * retreatVectorLength);
            Vector3 direction1 = new Vector3(-forwardVec.z * retreatVectorLength, 0,  forwardVec.x * retreatVectorLength);
            if (Random.Range(0, 100) % 2 == 0)
                _currentTarget = transform.position + direction0;
            else
                _currentTarget = transform.position + direction1;
        }
    }

    private void FireMissile()
    {
        _weaponsModule?.FireMissile();
        _attackRepeatCd = _attackRepeatDelay;
        CurrentBehavior = EnemyBehavior.Retreating; // retreat after the attack
    }

    private void RammingAttack()
    {
        _movementModule?.UseAfterburner();
        _weaponsModule?.toggleRam(true);
    }

    private void GizmoColorByBehavior()
    {
        switch (_currentBehavior)
        {
            case EnemyBehavior.None:
                Gizmos.color = Color.white;
                break;
            case EnemyBehavior.AttackFromDistance:
                Gizmos.color = Color.red;
                break;
            case EnemyBehavior.CloseQuatersAttack:
                Gizmos.color = Color.yellow;
                break;
            case EnemyBehavior.Retreating:
                Gizmos.color = Color.blue;
                break;
            case EnemyBehavior.Circling:
                Gizmos.color = Color.magenta;
                break;
        }
    }

    // renders collider+behavior
    private void OnDrawGizmos()
    {
        GizmoColorByBehavior();
        var bodyCollider = GetComponentInChildren<SphereCollider>();
        Gizmos.DrawWireSphere(bodyCollider.transform.position, bodyCollider.radius);
    }

    // renders current target when selected + behavior
    private void OnDrawGizmosSelected()
    {
        GizmoColorByBehavior();
        Gizmos.DrawCube(_currentTarget, Vector3.one);
    }
}
