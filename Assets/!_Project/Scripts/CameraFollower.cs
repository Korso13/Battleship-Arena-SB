using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    [SerializeField, Range(5f, 40f)]
    private float _radius = 5f;

    [SerializeField] Vector3 _cameraSpringArmDirection = Vector3.up;
    [SerializeField] private bool _isFollowingPlayer = true;

    public bool IsFollowingPlayer {get => _isFollowingPlayer; set => _isFollowingPlayer = value;}

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void LateUpdate()
    {
        if (!_isFollowingPlayer)
            return;

        Vector3 position = _playerTransform.position + _cameraSpringArmDirection * _radius;
        transform.position = position;
    }
}
