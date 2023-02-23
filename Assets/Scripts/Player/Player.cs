using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]

    #region Serialized
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSensiblility;
    [SerializeField] private int _cooldownFrontalShoot;
    [SerializeField] private int _cooldownLateralShoot;

    [Header("UI Components")]
    [SerializeField] private Image _healthUI;
    [SerializeField] private Gradient _healthGradient;
    [SerializeField] private FollowBoat _uiHealthFollowing;

    [Header("Boat and Bullet")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private BoatCanon _boat;
    #endregion

    #region Non Serialized
    private bool _ableToShoot;
    private bool _end;

    private CancellationTokenSource _tokenSource;
    #endregion

    private void Awake()
    {
        _uiHealthFollowing.Initiate(transform);
    }

    void Start()
    {
        _tokenSource = new CancellationTokenSource();

        _ableToShoot = true;
        _end = false;

        ChangeHealthValue(100);
    }

    private void OnEnable()
    {
        Events.onHitPlayerBoat += HandleHit;
        Events.onGameEnded += HandleEnd;
    }

    private void OnDisable()
    {
        Events.onHitPlayerBoat -= HandleHit;
        Events.onGameEnded -= HandleEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (_end) return;

        Movement();

        if (!_ableToShoot) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FrontalShoot();
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.RightShift))
        {
            LateralShoot(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            LateralShoot(false);
        }
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (y > 0) transform.Translate(Vector3.up * (_movementSpeed) * y * Time.deltaTime);
        else if (y == 0) transform.Translate(Random.insideUnitCircle * (0.2f) * Time.deltaTime);

        transform.Rotate(transform.forward * (-_rotationSensiblility) * x * Mathf.Max(y - 0.15f, 0.5f) * Time.deltaTime);
    }

    private async void FrontalShoot()
    {
        var canon = _boat.GetFrontCanon();
        if (canon == null) return;

        var bullet = Instantiate(_bulletPrefab, canon.position, Quaternion.identity);

        bullet.Instantited(transform.GetChild(0).gameObject, canon.forward);

        Events.onPlayerShoot(_cooldownFrontalShoot);
        _ableToShoot = false;

        await Task.Delay(1000 * _cooldownFrontalShoot);
        if (_tokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    private async void LateralShoot(bool right)
    {
        Transform[] canons;
        if (!right) canons = new Transform[] { _boat.GetFirstLeftCanon(), _boat.GetSecondLeftCanon(), _boat.GetThirdLeftCanon() };
        else canons = new Transform[] { _boat.GetFirstRightCanon(), _boat.GetSecondRightCanon(), _boat.GetThirdRightCanon() };
        int canonsAvailable = 0;

        for (int i = 0; i < canons.Length; i++)
        {
            if (canons[i] == null) continue;

            canonsAvailable++;
            var bullet = Instantiate(_bulletPrefab, canons[i].position, Quaternion.identity);
            bullet.Instantited(transform.GetChild(0).gameObject, canons[i].forward);
        }

        _ableToShoot = !(canonsAvailable > 0);

        if (_ableToShoot) return;
        Events.onPlayerShoot(_cooldownLateralShoot);

        await Task.Delay(1000 * _cooldownLateralShoot);
        if (_tokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    private void HandleHit(float factor, Vector3 hitPosition)
    {
        if (_health < 0) return;

        ChangeHealthValue(-(int)(15 * factor));

        if (_health < 0) Die();
        else Damage(hitPosition);
    }

    private void ChangeHealthValue(int valueToAdd)
    {
        _health += valueToAdd;
        HealthUISituation();
        _boat.ChangeBoatAccordingToHealth((float)_health / 100);
    }

    private void HealthUISituation()
    {
        if (_healthUI == null) return;

        float percentage = (float)_health / 100;

        _healthUI.fillAmount = percentage;
        _healthUI.color = _healthGradient.Evaluate(percentage);
    }

    private void Damage(Vector3 hitPosition)
    {
        bool justWood = Random.Range(0, 3) == 1 && _health > 60;

        if (justWood)
        {
            SpreadWood(hitPosition);
        }
        else
        {
            var pieceRemoved = _boat.PickRandomPieceWhenDamaged();

            if (pieceRemoved == null) SpreadWood(hitPosition);
            else pieceRemoved.Impulse();
        }
    }

    private void SpreadWood(Vector3 hitPosition)
    {
        int quantityOfWoods = Random.Range(2, 6);

        for (int i = 0; i < quantityOfWoods; i++)
        {
            var wood = Instantiate(_boat.WoodPrefab, hitPosition, Quaternion.identity);
            wood.Impulse();
        }
    }

    private void Die()
    {
        Events.OnGameEnded();

        _healthUI.transform.parent.gameObject.SetActive(false);
        _boat.DyingAnimation();
    }

    private void HandleEnd() => _end = true;

    public void DestroyObject() => Destroy(gameObject);
}
