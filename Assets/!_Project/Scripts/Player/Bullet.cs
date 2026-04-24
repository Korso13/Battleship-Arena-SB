using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    [SerializeField] private float _timeout = 5f;

    private void Start()
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = transform.forward * _speed;
        }
        else
            Debug.LogWarning("No rigidbody found");

        StartCoroutine(AutoDestruct());
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(_timeout);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If collision object is not DamageTaker - auto-destroy bullet
        if (collision.rigidbody == null
            || !collision.rigidbody.gameObject.TryGetComponent<DamageTaker>(out _))
        {
            //if(collision.gameObject)
            //    Debug.Log($"[{gameObject.name}] collided with Non-DamageTaker {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }

}
