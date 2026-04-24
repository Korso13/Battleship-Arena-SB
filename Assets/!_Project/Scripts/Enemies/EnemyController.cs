using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]/*Serializer for debugging only*/ private List<Enemy> _enemies;
    [SerializeField] private DamageTaker _player;
    [SerializeField] private int _maxEnemiesAllowed = 10;
    [SerializeField] private int _maxAttackingEnemies = 5;

    [Range(0.01f, 3f)]
    [SerializeField] private float _enemyUpdateFrequency = 0.125f;

    [Range(10f, 50f)]
    [SerializeField] private float _enemyCirclingDistanceMin = 25f;
    [Range(10f, 50f)]
    [SerializeField] private float _enemyCirclingDistanceMax = 35f;

    int _attackingEnemies = 0;
    private Coroutine _enemyAgressionCoroutine;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        _enemyAgressionCoroutine = StartCoroutine(StartEnemyAggression());
    }

    private void OnDisable()
    {
        if (_enemyAgressionCoroutine != null)
            StopCoroutine(_enemyAgressionCoroutine);
    }

    private void Update()
    {

    }

    private IEnumerator StartEnemyAggression()
    {
        var updateDelay = new WaitForSeconds(_enemyUpdateFrequency);

        while (true)
        {
            if (_player == null || _player.IsDead())
            {
                _enemyAgressionCoroutine = null;
                yield break;
            }

            int attackingEnemies = 0;
            foreach (Enemy enemy in _enemies)
            {
                if (enemy == null || enemy.IsDead) continue;

                // Setting target for attacking enemies
                // doing it via EnemyController allows us modify their aggression if we require it
                UpdateEnemyTarget(enemy);

                // Setting behavior for enemies without goal
                if (enemy.CurrentBehavior == Enemy.EnemyBehavior.None
                    || enemy.CurrentBehavior == Enemy.EnemyBehavior.Circling)
                    UpdateEnemyBehavior(attackingEnemies, enemy);

                // counting attacking enemies
                if(enemy.CurrentBehavior == Enemy.EnemyBehavior.CloseQuatersAttack 
                    || enemy.CurrentBehavior == Enemy.EnemyBehavior.AttackFromDistance)
                {
                    attackingEnemies++;
                }
            }

            _attackingEnemies = attackingEnemies;
            yield return updateDelay;
        }
    }

    private void UpdateEnemyTarget(Enemy enemy)
    {
        if (enemy.CurrentBehavior == Enemy.EnemyBehavior.CloseQuatersAttack 
            || enemy.CurrentBehavior == Enemy.EnemyBehavior.AttackFromDistance)
        {
            float distToPlayer = (enemy.transform.position - _player.transform.position).magnitude;
            float overshootModifier = 
                distToPlayer > enemy.MissileAttackDistance ? 
                1f : 
                (distToPlayer / enemy.MissileAttackDistance);
            enemy.CurrentTarget =
                _player.transform.position
                + _player.transform.forward * enemy.PlayerTargetingOvershootMax * overshootModifier;
        }
        else if (enemy.CurrentBehavior == Enemy.EnemyBehavior.Circling)
        {
            Vector3 playerRight = _player.transform.right;
            Vector3 playerLeft = Vector3.Reflect(_player.transform.right, _player.transform.right);
            Vector3 fromPlayerOffset;
            Vector3 toEnemyVector = enemy.transform.position - _player.transform.position;
            if (Vector3.Dot(toEnemyVector.normalized, playerRight.normalized) > 0f)
                fromPlayerOffset = playerRight.normalized;
            else
                fromPlayerOffset = playerLeft.normalized;
            float circlingDist = Random.Range(_enemyCirclingDistanceMin, _enemyCirclingDistanceMax);
            enemy.CurrentTarget =
                _player.transform.position
                + Vector3.Scale(fromPlayerOffset, new Vector3(circlingDist, 0, circlingDist));
        }
    }

    private void UpdateEnemyBehavior(int attackingEnemies, Enemy enemy)
    {
        if (_attackingEnemies < _maxAttackingEnemies
            && attackingEnemies < _maxAttackingEnemies)
        {
            if (Random.Range(0, 100) % 2 == 0)
                enemy.CurrentBehavior = Enemy.EnemyBehavior.CloseQuatersAttack;
            else
                enemy.CurrentBehavior = Enemy.EnemyBehavior.AttackFromDistance;
        }
        else
            enemy.CurrentBehavior = Enemy.EnemyBehavior.Circling;
    }

    public void RegisterEnemy(Enemy newEnemy)
    {
        _enemies.Add(newEnemy);
    }

    public void DeregisterEnemy(Enemy removeEnemy)
    {
        _enemies.Remove(removeEnemy);
    }

    public bool CanSpawnMoreEnemies()
    {
        return _enemies.Count < _maxEnemiesAllowed;
    }

    public Transform GetPlayerTracker()
    {
        return _player.transform;
    }
}
