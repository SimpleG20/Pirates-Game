using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("End")]
    [SerializeField] private GameObject _endScene;
    [SerializeField] private TextMeshProUGUI _scoreEndText;

    [Header("General")]
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private AudioSource _backgroundAudioSource;
    [SerializeField] private Animator _controlsAnimator;
    [SerializeField] private Loader _loader;

    private bool _paused;

    private SpawnManager _spawnManager;
    private ScoreManager _scoreManager;
    private CancellationTokenSource _tokenSource;

    private void OnEnable()
    {
        Events.onGameEnded += HandleEnd;
    }

    private void OnDisable()
    {
        Events.onGameEnded -= HandleEnd;
    }

    private void Start()
    {
        _scoreManager = GetComponent<ScoreManager>();
        _spawnManager = GetComponent<SpawnManager>();

        _tokenSource = new CancellationTokenSource();
    }

    public async void InitiateGameplay()
    {
        _backgroundAudioSource.Play();
        Transform player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity).transform;
        _virtualCamera.LookAt = player;
        _virtualCamera.Follow = player;

        await Task.Delay(1000);
        if (_tokenSource.IsCancellationRequested) return;

        _spawnManager.Initiate();
        Events.OnGameBeginning();
    }

    public void MusicSound()
    {
        if (_musicToggle.isOn) _backgroundAudioSource.mute = true;
        else _backgroundAudioSource.mute = false;
    }

    public void VisualizeHints(Toggle toggle)
    {
        _controlsAnimator.SetBool("Visualize", toggle.isOn);
    }

    public void TimeScale(float time)
    {
        if (time == 0)
        {
            _paused = true;
            _backgroundAudioSource.Pause();
        }
        else
        {
            _paused = false;
            _backgroundAudioSource.UnPause();
        }
        Events.OnPaused(_paused);
        Time.timeScale = time;
    }

    private void HandleEnd()
    {
        _backgroundAudioSource.Stop();
        _scoreEndText.text = _scoreManager.Score.ToString("0000");
        _endScene.SetActive(true);
    }

    public void LeaveGameplay()
    {
        PlayerPrefs.SetInt("SCORE", _scoreManager.Score);

        var loader = Instantiate(_loader);
        loader.Menu();
    }

    public void Restart()
    {
        _spawnManager.RestartBoats();

        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("Piece"))
        {
            Destroy(piece);
        }

        InitiateGameplay();
    }

    public void QuitGame()
    {
        Events.OnGameEnded();
    }

}
