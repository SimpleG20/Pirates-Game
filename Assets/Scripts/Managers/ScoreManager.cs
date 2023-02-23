using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;

    public int Score { get; private set; }

    private void OnEnable()
    {
        Events.onEnemyDefeated += HandleAddScore;
    }

    private void OnDisable()
    {
        Events.onEnemyDefeated -= HandleAddScore;
    }

    private void HandleAddScore(int type)
    {
        Score += type == 0 ? 200 : 175;
        _scoreText.text = $"Score : {Score.ToString("0000")}";
    }
}
