using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Loader _loader;

    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private Animator _configurations;
    [SerializeField] private TMP_InputField _matchTimeInput;
    [SerializeField] private TMP_InputField _spawnRateInput;

    private float _spawnRate;
    private int _matchTime;
    private int _score;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FIRST_TIME") == 0)
        {
            _spawnRate = 7f;
            _matchTime = 180;
            _score = 0;

            PlayerPrefs.SetFloat("SPAWN", _spawnRate);
            PlayerPrefs.SetInt("TIME", _matchTime);
            PlayerPrefs.SetInt("SCORE", _score);
            PlayerPrefs.SetInt("FIRST_TIME", 1);
        }
        else
        {
            _spawnRate = PlayerPrefs.GetFloat("SPAWN");
            _matchTime = PlayerPrefs.GetInt("TIME");
            _score = PlayerPrefs.GetInt("SCORE");
        }
        _matchTimeInput.placeholder.GetComponent<TextMeshProUGUI>().text = $"{_matchTime}";
        _spawnRateInput.placeholder.GetComponent<TextMeshProUGUI>().text = $"{_spawnRate}";
        _scoreText.text = $"Last Score: {_score.ToString("0000")}";
    }

    public void EnterConfigurations()
    {
        _configurations.SetBool("Show", true);
    }

    public void LeaveConfiguration()
    {
        _configurations.SetBool("Show", false);
    }

    public void ChangeSpawnRate()
    {
        float.TryParse(_spawnRateInput.text, out _spawnRate);
        if (_spawnRate < 3) { _spawnRate = 3; _spawnRateInput.text = "3"; }
        print(_spawnRate);
        PlayerPrefs.SetFloat("SPAWN", _spawnRate);
    }

    public void ChangeMatchTime()
    {
        int.TryParse(_matchTimeInput.text, out _matchTime);
        if (_matchTime < 60) { _matchTime = 60; _matchTimeInput.text = "60"; }
        print(_matchTime);
        PlayerPrefs.SetInt("TIME", _matchTime);
    }

    public void StartNewGame()
    {
        var loader = Instantiate(_loader);
        loader.StartGame();
    }


    [ContextMenu("First Time Enter")]
    private void ResetPrefs()
    {
        PlayerPrefs.SetInt("FIRST_TIME", 0);
    }
}
