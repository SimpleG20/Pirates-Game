using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform _enemySpawnParent;
    [SerializeField] private List<GameObject> _enemysShooterPrefab;
    [SerializeField] private List<GameObject> _enemysChaserPrefab;
    [SerializeField] private List<Transform> _positionsToSpawn;

    [SerializeField] private int _ShootersInstantiated;
    [SerializeField] private int _ChasersInstantiated;
    private int _enemysCount;

    private float _spawnRate;
    private bool _paused;
    private bool _end;

    private Player _player;
    private CancellationTokenSource _tokenSource;

    private void OnEnable()
    {
        Events.onPaused += HandlePause;
        Events.onEnemyDefeated += HandleEnemyDefeated;
        Events.onGameEnded += HandleEnd;
    }

    private void OnDisable()
    {
        Events.onPaused -= HandlePause;
        Events.onEnemyDefeated -= HandleEnemyDefeated;
        Events.onGameEnded -= HandleEnd;
    }

    private void Start()
    {
        _tokenSource = new CancellationTokenSource();
        _end = false;
    }


    public void Initiate()
    {
        _player = FindObjectOfType<Player>();

        _spawnRate = PlayerPrefs.GetFloat("SPAWN");

        SpawnEnemys();
    }

    private void HandlePause(bool value) => _paused = value;

    private void HandleEnemyDefeated(int type)
    {
        _enemysCount--;
        if (type == 0) _ShootersInstantiated--;
        else _ChasersInstantiated--;
    }

    private void HandleEnd() => _end = true;

    private async void SpawnEnemys()
    {
        if (_paused || _end) return;

        if (_enemysCount <= 10)
        {
            int randomEnemy = Random.Range(0, 2);
            
            Vector3 positionToSpawn = SetPositionToSpawn();
            InstantiateEnemy(randomEnemy, positionToSpawn);
        }

        await Task.Delay((int)(1000 * _spawnRate));
        if (_tokenSource.IsCancellationRequested || _paused || _end) return;

        SpawnEnemys();
    }

    private void InstantiateEnemy(int randomEnemy, Vector3 positionToSpawn)
    {
        if (randomEnemy == 0)
        {
            if (_ShootersInstantiated > 4)
            {
                Instantiate(_enemysChaserPrefab.PickRandomList(), positionToSpawn, Quaternion.identity, _enemySpawnParent);
                _ChasersInstantiated++;
            }
            else
            {
                Instantiate(_enemysShooterPrefab.PickRandomList(), positionToSpawn, Quaternion.identity, _enemySpawnParent);
                _ShootersInstantiated++;
            }

        }
        else
        {
            if (_ChasersInstantiated > 4)
            {
                Instantiate(_enemysShooterPrefab.PickRandomList(), positionToSpawn, Quaternion.identity, _enemySpawnParent);
                _ShootersInstantiated++;
            }
            else
            {
                Instantiate(_enemysChaserPrefab.PickRandomList(), positionToSpawn, Quaternion.identity, _enemySpawnParent);
                _ChasersInstantiated++;
            }
        }
        _enemysCount++;
    }

    private Vector3 SetPositionToSpawn()
    {
        float maxDistance = 0;
        var random = _positionsToSpawn.PickRandomList();

        if (random == null) return new Vector2(-10, 7);

        Vector3 positionToSpawn = random.position;

        foreach (Transform t in _positionsToSpawn)
        {
            float distance = Vector3.Distance(t.position, _player.transform.position);
            if (distance > maxDistance)
            {
                positionToSpawn = t.position;
                maxDistance = distance;
            }
        }

        positionToSpawn = new Vector3(positionToSpawn.x + Random.Range(-1f, 1f), positionToSpawn.y + Random.Range(-1f, 1f), 0);
        return positionToSpawn;
    }

    public void RestartBoats()
    {
        _enemySpawnParent.DeleteChildren();
        Destroy(_player.gameObject);
    }

}
