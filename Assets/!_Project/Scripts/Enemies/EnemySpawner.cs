using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _enemiesNode;
    [SerializeField] private float _productionDelay = 5f;
    [SerializeField] private bool _isLoggingTriggerZone = false;

    private Dictionary<Rigidbody, bool> bodiesInVicinity = new Dictionary<Rigidbody, bool>();
    
    static private int s_enemiesUidCounter = 0;

    private void OnEnable()
    {
        bodiesInVicinity.Clear();
        StartCoroutine(StartProduction());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartProduction()
    {
        while(true)
        {
            yield return new WaitForSeconds(_productionDelay);
            while(!TryToManifactureEnemy())
            {
                yield return null;
            }
        }
    }

    private bool TryToManifactureEnemy()
    {
        if (!CanManufacture())
        {
            if (_isLoggingTriggerZone)
                Debug.Log($"Can't spawn enemy - bodies inside perimeter: {bodiesInVicinity.Count}");
            return false;
        }

        Vector3 spawnPosition = _spawnPoint.position;
        Quaternion bulletDirection = _spawnPoint.rotation;
        var enemy = Instantiate(_enemyPrefab, spawnPosition, bulletDirection);
        enemy.transform.SetParent(_enemiesNode);
        enemy.gameObject.name = "Enemy_" + s_enemiesUidCounter++.ToString("D3");
        if (enemy.gameObject.TryGetComponent<Enemy>(out var enemyComp))
        {
            enemyComp.EnemyController = _enemyController;
            // todo: randomize some characteristics
            _enemyController.RegisterEnemy(enemyComp);
        }

        return true;
    }

    public bool CanManufacture()
    {
        return bodiesInVicinity.Count == 0 && _enemyController.CanSpawnMoreEnemies();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
            return;
        if(!bodiesInVicinity.ContainsKey(other.attachedRigidbody))
        {
            bodiesInVicinity.Add(other.attachedRigidbody, true);
            if (_isLoggingTriggerZone)
                Debug.Log($"Body {other.attachedRigidbody.gameObject.name} ENTERED Spawner detector! " +
                    $"Bodies in vicinity: {bodiesInVicinity}");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null)
            return;
        if (bodiesInVicinity.ContainsKey(other.attachedRigidbody))
        {
            bodiesInVicinity.Remove(other.attachedRigidbody);
            if (_isLoggingTriggerZone)
            Debug.Log($"Body {other.attachedRigidbody.gameObject.name} LEFT Spawner detector! " +
                $"Bodies in vicinity: {bodiesInVicinity}");
        }
    }
}
