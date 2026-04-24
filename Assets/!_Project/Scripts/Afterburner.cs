using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Afterburner : MonoBehaviour
{
    [SerializeField] private float _afterburnerDuration = 1.5f;
    [SerializeField] private float _afterburnerRecharge = 3.5f;
    [SerializeField] private float _afterburnerSpeedboost = 1.0f;
    [SerializeField] private float _afterburnerAccelerationBoost = 0.15f;
    [SerializeField] private AudioSource _boosterSound;
    [SerializeField] private PlayerUI _playerUI;

    public float SpeedBoost { get => _afterburnerSpeedboost; }
    public float AccelerationBoost { get => _afterburnerAccelerationBoost; }

    private float afterburnerTimeout = 0f;
    private float afterburnerRecharge = 0f;

    // Events
    public UnityEvent OnAfterburnerOff;
    public UnityEvent OnAfterburnerOn;

    // Update is called once per frame
    private void Update()
    {
        if (afterburnerRecharge > 0f)
        {
            afterburnerRecharge -= Time.deltaTime;
            if (afterburnerRecharge <= 0f && _playerUI)
                _playerUI.SetBoosterReady(true);
        }

        if (afterburnerTimeout > 0f)
        {
            afterburnerTimeout -= Time.deltaTime;
            if(afterburnerTimeout < 0f)
            {
                afterburnerRecharge = _afterburnerRecharge;
                afterburnerTimeout = 0f;
                OnAfterburnerOff.Invoke();
            }
        }
    }

    public bool CanUse()
    {
        return afterburnerRecharge <= 0f && afterburnerTimeout == 0f;
    }

    public bool TryUseAfterburner()
    {
        if(CanUse())
        {
            afterburnerTimeout = _afterburnerDuration;
            OnAfterburnerOn.Invoke();
            if(_boosterSound != null)
                _boosterSound.Play();
            if(_playerUI != null)
                _playerUI.SetBoosterReady(false);
            return true;
        }
        else
            return false;
    }

    public void StopAfterburner()
    {
        if (afterburnerTimeout <= 0f)
            return;

        afterburnerTimeout = 0f;
        afterburnerRecharge = _afterburnerRecharge;
        OnAfterburnerOff.Invoke();
    }
}
