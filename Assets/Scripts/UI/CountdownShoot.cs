using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownShoot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private Image _fillSource;

    private float _time;
    private float _maxTime;
    private bool _startedCountdown;

    private void OnEnable()
    {
        Events.onPlayerShoot += HandleOnPlayerShoot;
    }

    private void OnDisable()
    {
        Events.onPlayerShoot -= HandleOnPlayerShoot;
    }

    private void Start()
    {
        SetUis();
    }

    private void Update()
    {
        if (!_startedCountdown) return;

        _time -= Time.deltaTime;
        if (_time < 0)
        {
            _time = 0;
            _startedCountdown = false;
        }

        SetUis();
    }

    private void HandleOnPlayerShoot(float time)
    {
        _time = time;
        _maxTime = _time;
        _startedCountdown = true;
    }

    private void SetUis()
    {
        if (_startedCountdown)
        {
            _countdownText.text = _time.ToString("F1").Replace(',', '.');
            _fillSource.fillAmount = 1 - (_time / _maxTime);
        }
        else
        {
            _countdownText.text = "0";
            _fillSource.fillAmount = 0;
        }
    }
}
