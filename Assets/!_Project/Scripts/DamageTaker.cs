using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageTaker : MonoBehaviour
{
    [SerializeField] private PlayerUI _playerUI;
    //[SerializeField] private Collider _hitCollider;
    [SerializeField] private ParticleSystem _onDeathParticleFx;
    [SerializeField] private GameObject _bodyToDestroy;
    [SerializeField] private AudioSource _deathSound;
    [SerializeField] private int _hitPointsMax = 100;
    [SerializeField] private float _gracePeriod = 0.15f;
    [SerializeField] private bool _godMode = false;
    public UnityEvent OnDeath;

    private int hitPointsCurrent = 100;
    private float gracePeriodTimer = 0f;

    private void Start()
    {
        hitPointsCurrent = _hitPointsMax;
        if (_playerUI != null)
        {
            _playerUI.MaxHP = _hitPointsMax;
            _playerUI.UpdateHPValue(hitPointsCurrent, true);
        }
    }

    private void OnEnable()
    {
        _bodyToDestroy?.SetActive(true);
    }

    private void Update()
    {
        if (gracePeriodTimer > 0f)
            gracePeriodTimer -= Time.deltaTime;
    }

    public bool IsDead()
    {
        return hitPointsCurrent <= 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsDead())
            return;
        //if(other.attachedRigidbody != null && !other.isTrigger) 
        //    Debug.Log($"[{gameObject.name}]OnTriggerEnter - {other.attachedRigidbody.gameObject.name}");
        

        if(other.attachedRigidbody != null 
            && other.attachedRigidbody.gameObject.TryGetComponent<DamageMaker>(out var damageMaker))
        {
            //Debug.Log($"[{gameObject.name}] took {damageMaker.Damage} damage from {other.attachedRigidbody.gameObject.name}");
            TakeDamage(damageMaker.Damage);
            
            //Destroy(other.gameObject);
            damageMaker.SafeDestroy();
        }
    }

    private void CheckForScoring()
    {
        if(TryGetComponent<ScoringObject>(out var scoreSource))
        {
            GameStats.AddScore(scoreSource.ScoreValue);
            if (scoreSource.IsEnemy)
                GameStats.RegisterKill();
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead() || _godMode || gracePeriodTimer > 0f) return;
        gracePeriodTimer = _gracePeriod;
        UpdateHitPoints(-damage);
    }

    private void UpdateHitPoints(int delta)
    {
        hitPointsCurrent += delta;
        Debug.Log($"[{gameObject.name}] took {-delta} damage. Remaining HP: {hitPointsCurrent}. Is Dead: {IsDead()}");
        if (IsDead())
        {
            _deathSound?.Play();
            CheckForScoring();
            OnDeath.Invoke();
            _bodyToDestroy?.SetActive(false);
            _onDeathParticleFx?.Play();
            StartCoroutine(
                ScheduleDestruction(
                    _onDeathParticleFx != null 
                    ? _onDeathParticleFx.main.duration
                    : 0f)
                );
        }
        _playerUI?.UpdateHPValue(hitPointsCurrent);
    }
    public void SafeDestroy()
    {
        StartCoroutine(ScheduleDestruction(0f));
    }

    private IEnumerator ScheduleDestruction(float seconds)
    {
        //Debug.Log($"[DamageTaker][{gameObject.name}] ScheduleDestruction");
        if (gameObject.TryGetComponent<Enemy>(out var enemyComp))
        {
            enemyComp.EnemyController.DeregisterEnemy(enemyComp);
        }

        if (seconds > 0)
            yield return new WaitForSeconds(seconds);
        // Hack to prompt TriggerExits in EnemySpawners
        gameObject.transform.position += new Vector3(-100, -100, -100);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (gameObject.TryGetComponent<Rigidbody>(out var rb))
            rb.detectCollisions = false;
        yield return new WaitForEndOfFrame();
        //Debug.Log($"[DamageTaker][{gameObject.name}] Destroying...");
        Destroy(gameObject);
    }
}
