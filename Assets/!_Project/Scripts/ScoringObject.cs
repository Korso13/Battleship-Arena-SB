using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringObject : MonoBehaviour
{
    [SerializeField] private int _scoreValue = 1; 
    [SerializeField] private bool _increasesEnemyCount = false; 
    public int ScoreValue {get => _scoreValue;}
    public bool IsEnemy {get => _increasesEnemyCount; }
}
