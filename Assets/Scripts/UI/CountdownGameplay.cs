using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownGameplay : MonoBehaviour
{
    private float _time;
    public float CurrentTime
    {
        get => _time;
        set
        {
            _time = value;
            if (_time <= 0) 
            {
                _isCountdown = false;
                Events.OnGameEnded(); 
                return; 
            }
            SetUI();
        }
    }

    private bool _isCountdown;

    [SerializeField] private TextMeshProUGUI _timeText;

    private void OnEnable()
    {
        Events.onPaused += HandlePause;
        Events.onGameBeginning += HandleBeginning;
        Events.onGameEnded += HandleEnd;
    }

    private void OnDisable()
    {
        Events.onPaused -= HandlePause;
        Events.onGameBeginning -= HandleBeginning;
        Events.onGameEnded -= HandleEnd;
    }

    private void Start()
    {
        CurrentTime = PlayerPrefs.GetInt("TIME");
        SetUI();
    }

    void Update()
    {
        if (!_isCountdown) return;

        _time -= Time.deltaTime;
        SetUI();
    }

    private void HandleBeginning() => _isCountdown = true;

    private void HandleEnd() => _isCountdown = false;

    private void SetUI()
    {
        var minute = Mathf.Floor(_time / 60);
        var seconds = _time % 60;
        _timeText.text = $"{minute.ToString("00")} : {seconds.ToString("00")}";
    }

    private void HandlePause(bool pause)
    {
        _isCountdown = !pause;
    }
}
