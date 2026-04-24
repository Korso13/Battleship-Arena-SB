using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapons : MonoBehaviour
{
    [SerializeField] private Transform _weaponTube;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private Transform _enemiesNode;
    [SerializeField] private ParticleSystem _ramAttackFx;
    [SerializeField] private AudioSource _ramAttackSound;
    [SerializeField] private int _ramAttackDamage = 10;

    private EnemyController _enemyController;
    private bool _isRammingEnabled = false;
    private Vector3 _currentTarget = new Vector3(0, 0, 0);
    // Updates target for the EnemyMissiles
    public Vector3 CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    public bool IsRamming { get => _isRammingEnabled; set => _isRammingEnabled = value; }

    static private int s_missilesUidCounter = 0;

    // Start is called before the first frame update
    private void Start()
    {
        _enemyController = gameObject.GetComponent<Enemy>()?.EnemyController;
    }

    public void FireMissile()
    {
        Vector3 spawnPosition = _weaponTube.position;
        Quaternion bulletDirection = _weaponTube.rotation;
        var missile = Instantiate(_missilePrefab, spawnPosition, bulletDirection);
        missile.transform.SetParent(transform.parent);
        missile.gameObject.name = "Missile_" + s_missilesUidCounter++.ToString("D3");
        if(missile.TryGetComponent<Missile>(out var missileComp))
        {
            missileComp.PlayerTracker = _enemyController.GetPlayerTracker(); // lets missile handle tracking by itself
            missileComp.CurrentTarget = _currentTarget;
        }
    }

    public void toggleRam(bool isRammingEnabled)
    {
        IsRamming = isRammingEnabled;
        if (isRammingEnabled)
        {
            _ramAttackSound?.Play();
            _ramAttackFx?.Play();
        }
        else
            _ramAttackFx?.Stop();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_isRammingEnabled) return;

        if(other.attachedRigidbody != null 
            &&  other.attachedRigidbody.TryGetComponent<PlayerWeapons>(out _)
            && other.attachedRigidbody.TryGetComponent<DamageTaker>(out var playerHpComponent))
        {
            //Debug.Log($"{gameObject.name} rammed Player!");
            playerHpComponent.TakeDamage(_ramAttackDamage);
        }
    }
}
