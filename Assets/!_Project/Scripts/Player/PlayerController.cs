using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        if(TryGetComponent<DamageTaker>(out var DamageTaker))
        {
            DamageTaker.OnDeath.AddListener(() => SceneSwitcher.LoadEndscreen());
        }
    }
}
