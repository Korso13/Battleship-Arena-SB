using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : SingletonBehaviour<GameStats>
{
    public struct GameStatsInfo
    {
        public float gameTime;
        public int minutes;
        public int seconds;
        public int score;
        public int killedEnemies;
    }

    [SerializeField] private float timeToScoreConversionRate = 1.0f;

    private bool isGameOn = false;
    private int killedEnemies = 0;
    private int score = 0;
    private float gameplayTimer = 0f;
    private Coroutine gameTimerCorutine;
    //static private bool dndCalled = false;

    public float GameplayTimer { get => gameplayTimer; }

    //private void Awake()
    //{
    //    //if(!dndCalled)
    //    //{
    //    //    DontDestroyOnLoad(gameObject);
    //    //    dndCalled = true;
    //    //}
    //}

    private void Start()
    {
        Debug.Log("[GameStats]Start");
        //Restart(); // otherwise timer won't work if we start with Gameplay scene. SHould not interfere in other cases
    }

    static private IEnumerator StartGameTimer()
    {
        Debug.Log("[GameStats]StartGameTimer");
        while (Singleton.isGameOn)
        {
            yield return new WaitForSeconds(1f);
            Singleton.gameplayTimer += 1f;
        }
    }

    static public void Restart()
    {
        Singleton.killedEnemies = 0;
        Singleton.score = 0;
        Singleton.gameplayTimer = 0f;
        Singleton.isGameOn = true;
        Singleton.gameTimerCorutine = Singleton.StartCoroutine(StartGameTimer());
    }

    static public void Pause()
    {
        Singleton.isGameOn = false;
        if(Singleton.gameTimerCorutine  != null)
            Singleton.StopCoroutine(Singleton.gameTimerCorutine);
        Singleton.gameTimerCorutine = null;
    }

    static public void RegisterKill()
    {
        Singleton.killedEnemies++;
    }

    static public void AddScore(int scoreToAdd)
    {
        if(scoreToAdd < 0) return;
        Singleton.score += scoreToAdd;
    }

    static public GameStatsInfo GetStats()
    {
        GameStatsInfo stats = new GameStatsInfo();
        stats.gameTime = Singleton.gameplayTimer;
        stats.minutes = (int)Mathf.Floor(Singleton.gameplayTimer / 60f);
        stats.seconds = (int)Mathf.Floor(Singleton.gameplayTimer - stats.minutes * 60f);
        stats.score = Singleton.score + (int)Mathf.Floor(Singleton.gameplayTimer * Singleton.timeToScoreConversionRate);
        stats.killedEnemies = Singleton.killedEnemies;
        return stats;
    }
}
