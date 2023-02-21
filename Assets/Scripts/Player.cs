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
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSensiblility;
    [SerializeField] private int _cooldownFrontalShoot;
    [SerializeField] private int _cooldownLateralShoot;

    [Header("UI Components")]
    [SerializeField] private Image HealthUI;
    [SerializeField] private Gradient HealthGradient;

    [Header("Boat and Bullet")]
    [SerializeField] private Bullet BulletPrefab;
    [SerializeField] private Boat _boat = new Boat();
    #endregion

    #region Non Serialized
    private int _health;
    private int _playerLayer = 6;
    private bool _ableToShoot;

    private CancellationTokenSource _tokenSource;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _tokenSource = new CancellationTokenSource();
        _ableToShoot = true;
        _health = 100;

        HealthUISituation();
    }

    private void OnEnable()
    {
        Events.onHitPlayerBoat += HandleHit;
    }

    private void OnDisable()
    {
        Events.onHitPlayerBoat -= HandleHit;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (!_ableToShoot) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            FrontalShoot();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LateralShoot();
        }
    }

    private void HealthUISituation()
    {
        if (HealthUI == null) return;

        HealthUI.fillAmount = _health;
        HealthUI.color = HealthGradient.Evaluate(_health / 100);
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (y > 0) transform.Translate(Vector3.up * (-_movementSpeed) * y * Time.deltaTime);
        else if (y == 0) transform.Translate(Vector3.up * (-0.05f) * Time.deltaTime);

        transform.Rotate(Vector3.forward * (-_rotationSensiblility) * x * (1.5f - y) * Time.deltaTime);
    }

    private async void FrontalShoot()
    {
        var canon = _boat.GetFrontCanon();
        if (canon == null) return;

        var bullet = Instantiate(BulletPrefab, canon.position, Quaternion.identity);

        bullet.Instantited(_playerLayer, canon.forward);
        _ableToShoot = false;

        await Task.Delay(1000 * _cooldownFrontalShoot);
        if (_tokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    private async void LateralShoot()
    {
        var canons = new Transform[] { _boat.GetFirstLateralCanon(), _boat.GetSecondLateralCanon(), _boat.GetThirdLateralCanon() };

        for (int i = 0; i < canons.Length; i++)
        {
            if (canons[i] == null) continue;

            var bullet = Instantiate(BulletPrefab, canons[i].position, Quaternion.identity);
            bullet.Instantited(_playerLayer, canons[i].forward);
        }
        _ableToShoot = false;

        await Task.Delay(1000 * _cooldownLateralShoot);
        if (_tokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    private void HandleHit(float factor, Vector3 hitPosition)
    {
        if (_health < 0) return;

        _health -= (int)(15 * factor);
        HealthUISituation();

        if (_health < 0) Die();
        else
        {
            Damage(hitPosition);
            _boat.ChangeBoatAccordingToHealth(_health);
        }
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
            var pieceRemoved = _boat.RemoveRandomPieceWhenDamaged();

            if (pieceRemoved == false) SpreadWood(hitPosition);
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

    private async void Die()
    {
        _boat.DyingAnimation();
        await Task.Delay(2250);

        if (_tokenSource.IsCancellationRequested) return;
        Destroy(gameObject);
    }
}
