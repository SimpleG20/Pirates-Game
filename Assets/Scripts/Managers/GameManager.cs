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
    [SerializeField] private Animator _hintAnimator;
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
        Transform player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity).transform;
        _virtualCamera.LookAt = player;
        _virtualCamera.Follow = player;

        await Task.Delay(1000);
        if (_tokenSource.IsCancellationRequested) return;

        _spawnManager.Initiate();
        Events.OnGameBeginning();
    }

    public void VisualizeHints(Toggle toggle)
    {
        _hintAnimator.SetBool("Visualize", toggle.isOn);
    }

    public void TimeScale(float time)
    {
        if (time == 0)
        {
            _paused = true;
        }
        else
        {
            _paused = false;
        }
        Events.OnPaused(_paused);
        Time.timeScale = time;
    }

    public void HandleEnd()
    {
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

}
