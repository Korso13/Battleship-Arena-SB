using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EndscreenResults : MonoBehaviour
{
    [Serializable]
    public struct ResultVariant
    {
        public int MinScoreForResult;
        public string ResultHeaderText;
    }
    public class ResultVariantSorter : Comparer<ResultVariant>
    {
        public override int Compare(ResultVariant x, ResultVariant y)
        {
            return y.MinScoreForResult.CompareTo(x.MinScoreForResult);
        }
    }

    [SerializeField] private List<ResultVariant> _resultVariants = new List<ResultVariant>(3);
    [SerializeField] private TMPro.TextMeshProUGUI _resultText;
    [SerializeField] private TMPro.TextMeshProUGUI _timeText;
    [SerializeField] private TMPro.TextMeshProUGUI _killsText;
    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;

    // Start is called before the first frame update
    private void Start()
    {
        GameStats.GameStatsInfo gameStats = GameStats.GetStats();

        // Sorting ResultVariants by MinScoreForResult
        _resultVariants.Sort(new ResultVariantSorter());
        var result = from resultVariant in _resultVariants
                     where resultVariant.MinScoreForResult <= gameStats.score
                     select resultVariant.ResultHeaderText;

        _resultText.text = result.ElementAt(0);
        _timeText.text = gameStats.minutes.ToString("D2") + ":" + gameStats.seconds.ToString("D2");
        _killsText.text = gameStats.killedEnemies.ToString();
        _scoreText.text = gameStats.score.ToString();
    }
}
