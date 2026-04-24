using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private DamageMaker _damageMakerModule;
    [SerializeField] private DamageTaker _damageTakerModule;
    [SerializeField] private MovableObject _movementModule;
    [SerializeField] private AudioSource _missileSound;
    [Range (0.01f, 10f)]
    [SerializeField] private float _playerTargetPrediction = 1.5f;
    [Range (0.25f, 5f)]
    [SerializeField] private float _retargetFrequency = 2.0f;
    [SerializeField] private float _timeout = 5f;

    private Vector3 _currentTarget = new Vector3(0, 0, 0);

    public Vector3 CurrentTarget 
    { 
        get => _currentTarget; 
        set
        {
            _currentTarget = value;
            if (PlayerTracker != null)
                _currentTarget += PlayerTracker.forward * _playerTargetPrediction;
        }
    }
    public Transform PlayerTracker;


    private void Start()
    {
        StartCoroutine(AutoDestruct());
        StartCoroutine(RetargetingCourutine());
        _missileSound?.Play();
    }

    private void FixedUpdate()
    {
        _movementModule?.TurnToPoint(_currentTarget);
    }

    private void SafeDestroy()
    {
        StopAllCoroutines();
        if (_damageTakerModule != null)
        {
            _damageTakerModule.SafeDestroy();
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Failed to safely destroy - DamageTaker component not found!");
            Destroy(gameObject);
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(_timeout);
        SafeDestroy();
    }

    private IEnumerator RetargetingCourutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(_retargetFrequency);
            CurrentTarget = PlayerTracker.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null
            || !collision.rigidbody.gameObject.TryGetComponent<DamageTaker>(out _))
        {
            if(collision.gameObject)
                Debug.Log($"[{gameObject.name}] Missile collided with Non-DamageTaker {collision.gameObject.name}. Destroying");
            
            SafeDestroy();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(_currentTarget, Vector3.one);
    }
}
