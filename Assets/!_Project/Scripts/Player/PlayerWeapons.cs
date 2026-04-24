using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private PlayerUI _playerUI;
    [SerializeField] private List<Transform> _weapons;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private AudioSource _shootSound;
    [SerializeField] private float _refireDelay = 0.5f;
    [SerializeField] private float _shotEnergyCost = 1f;
    [SerializeField] private float _maxEnergy = 14f;
    [SerializeField] private float _energyRegenerationRate = 2f;

    private float _energy = 0f;
    private float _weaponsCooldown = 0f;

    private void Start()
    {
        _energy = _maxEnergy;
        if(_playerUI != null)
        {
            _playerUI.MaxEnergy = _maxEnergy;
            _playerUI.UpdateEnergyValue(_energy);
        }
    }

    private void Update()
    {
        if (_weaponsCooldown > 0f)
            _weaponsCooldown -= Time.deltaTime;
        if (_energy < _maxEnergy)
        {
            _energy = Mathf.Clamp(
                _energy + _energyRegenerationRate * Time.deltaTime,
                0,
                _maxEnergy);
            _playerUI?.UpdateEnergyValue(_energy);
        }
    }

    public bool CanShoot()
    {
        return _weaponsCooldown <= 0f && _energy > _shotEnergyCost * _weapons.Count;
    }

    public void TryShootWeapons()
    {
        if (!CanShoot()) return;

        _shootSound?.Play();
        _weaponsCooldown = _refireDelay;
        _energy -= _shotEnergyCost * _weapons.Count;
        _playerUI?.UpdateEnergyValue(_energy);

        foreach (Transform weapon in _weapons)
        {
            Vector3 spawnPosition = weapon.position;
            Quaternion bulletDirection = weapon.rotation;
            var bullet = Instantiate(_projectilePrefab, spawnPosition, bulletDirection);
            bullet.transform.SetParent(transform.parent);
        }
    }
}
