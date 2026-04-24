using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameStats;

public class PlayerUI : MonoBehaviour
{
    //[SerializeField] private TMPro.TextMeshProUGUI _maxHpUi;
    //[SerializeField] private TMPro.TextMeshProUGUI _currentHpUi;
    [SerializeField] private UnityEngine.UI.Slider _hpBar;
    [SerializeField] private TMPro.TextMeshProUGUI _maxEnergyUi;
    [SerializeField] private TMPro.TextMeshProUGUI _currentEnergyUi;
    [SerializeField] private TMPro.TextMeshProUGUI _playtimeUi;
    [SerializeField] private UnityEngine.UI.Image _damageVignette;
    [SerializeField] private UnityEngine.UI.Image _boosterStateIco;

    private int maxHp = 0;
    private float maxEnergy = 0f;
    private Tweener vignetteTween;

    public int MaxHP
    {
        get => maxHp;
        set
        {
            maxHp = value;
            //if(_maxHpUi != null)
            //    _maxHpUi.text = maxHp.ToString();
            if(_hpBar != null)
                _hpBar.value = 1f;
        }
    }
    public float MaxEnergy 
    { 
        get => maxEnergy;
        set
        {
            maxEnergy = value;
            if (_maxEnergyUi != null)
                _maxEnergyUi.text = maxEnergy.ToString("F0");
        }
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        if (_damageVignette != null)
            _damageVignette.color = new Color(1, 1, 1, 0);
        StartCoroutine(StartGameTimer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.CompleteAll();
    }

    private IEnumerator StartGameTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GameStatsInfo stats = GameStats.GetStats();
            int minutes = stats.minutes;
            int seconds = stats.seconds;
            string timerText = minutes.ToString("D2") + ":" + seconds.ToString("D2");
            if (_playtimeUi != null)
                _playtimeUi.text = timerText;
        }
    }

    public void UpdateHPValue(int newHPValue, bool noVignetteAnim = false)
    {
        //if (_currentHpUi != null)
        //    _currentHpUi.text = newHPValue.ToString();
        if (_hpBar != null)
            _hpBar.value = (float)newHPValue / (float)maxHp;

        // vignette animation
        if (!noVignetteAnim && _damageVignette != null && vignetteTween == null)
        {
            vignetteTween = DOVirtual.Color(
                new Color(1, 1, 1, 0),
                Color.white,
                0.18f,
                (Color col) => _damageVignette.color = col);
            vignetteTween.SetLoops(6, LoopType.Yoyo);
            vignetteTween.onComplete = () => vignetteTween = null;
        }
    }

    public void UpdateEnergyValue(float newEnergyValue)
    {
        if (_currentEnergyUi != null)
            _currentEnergyUi.text = newEnergyValue.ToString("F0");
    }

    public void SetBoosterReady(bool isReady)
    {
        if (_boosterStateIco == null) return;

        if(isReady)
            _boosterStateIco.color = Color.white;
        else
            _boosterStateIco.color = new Color(0,0,0,0.5f);
    }
}
