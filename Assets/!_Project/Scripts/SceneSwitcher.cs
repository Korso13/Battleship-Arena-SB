using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : SingletonBehaviour<SceneSwitcher>
{
    [SerializeField] private string _mainMenuScene = "01_MainMenu";
    [SerializeField] private string _gameLevelScene = "02_GameLevel";
    [SerializeField] private string _endscreenScene = "03_EndSceen";
    //[SerializeField] private string _winScreenScene = "04_FinalScene";

    private void Start()
    {

    }

    static public void LoadMainMenu()
    {
        Debug.Log("[SceneSwitcher]To Main Menu");
        GameStats.Pause();
        SceneManager.LoadScene(Singleton._mainMenuScene);
    }

    static public void LoadGameLevel()
    {
        Debug.Log("[SceneSwitcher]To Gameplay");
        GameStats.Restart();
        SceneManager.LoadScene(Singleton._gameLevelScene);
    }

    static public void LoadEndscreen()
    {
        Debug.Log("[SceneSwitcher]To Endscreen");
        GameStats.Pause();
        SceneManager.LoadScene(Singleton._endscreenScene);
    }

    //public void LoadWinscreen()
    //{
    //    SceneManager.LoadScene(_winScreenScene);
    //}
}
