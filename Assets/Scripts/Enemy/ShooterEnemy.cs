using System.Threading.Tasks;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    [Header("Shooter Variables")]
    [SerializeField] private int _cooldownFrontalShoot;
    [SerializeField] private int _cooldownLateralShoot;

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private BoatCanon _boat;

    private int _directionToShoot;
    private int _randomDirectionToRotate;
    private bool _ableToShoot;
    private bool _rotatingToShoot;

    void Start()
    {
        Initialization();
        Agent.speed = MovementSpeed;

        _ableToShoot = true;
    }

    void Update()
    {
        if (_end) return;

        if (Vector3.Distance(Player.transform.position, transform.position) > FieldVision)
        {
            Move();
            RotateDirectlyToPlayer();
        }
        else
        {
            Agent.velocity = Vector3.zero;
            if (!_ableToShoot) return;

            VerifyPlayerPresence();
            if (_rotatingToShoot) 
                transform.Rotate(Vector3.right * RotationSensibility * _randomDirectionToRotate * Mathf.Max(Agent.velocity.magnitude - 0.15f, 0.5f) * Time.deltaTime);
        }
    }


    private void VerifyPlayerPresence()
    {
        int canonsAvailables = 0;

        foreach (Canon canon in _boat.GetCanons())
        {
            if (canon.IsPlayerWithinSight(FieldVision))
            {
                _directionToShoot = canon.CanonDirection;
                _rotatingToShoot = false;
                Shoot();
                canonsAvailables++;
            }
        }

        if (canonsAvailables >= 1) return;

        RotateToShoot();
    }

    private Transform[] GetCanonsToStartShooting()
    {
        Transform[] canons;
        if (_directionToShoot == 1) canons = new Transform[] { _boat.GetFrontCanon() };
        else if (_directionToShoot == 2) canons = new Transform[] { _boat.GetFirstLeftCanon(), _boat.GetSecondLeftCanon(), _boat.GetThirdLeftCanon() };
        else canons = new Transform[] { _boat.GetFirstRightCanon(), _boat.GetSecondRightCanon(), _boat.GetThirdRightCanon() };
        return canons;
    }

    private void InstantiateBullets(Transform[] canons)
    {
        for (int i = 0; i < canons.Length; i++)
        {
            if (canons[i] == null) continue;

            var bullet = Instantiate(_bulletPrefab, canons[i].position, Quaternion.identity);
            bullet.Instantited(transform.GetChild(0).gameObject, canons[i].right);
        }
    }

    public async void Shoot()
    {
        Transform[] canons = GetCanonsToStartShooting();
        InstantiateBullets(canons);
        _ableToShoot = false;

        int timeToShootAgain = _directionToShoot == 1 ? _cooldownFrontalShoot : _cooldownLateralShoot;

        await Task.Delay(1000 * timeToShootAgain);
        if (TokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    private void RotateToShoot()
    {
        if (!_rotatingToShoot)
        {
            if (_boat.GetCanons().Count == 0) _randomDirectionToRotate = 0;
            else
            {
                _randomDirectionToRotate = _boat.GetCanons()[0].CanonDirection < 3 ? 1 : -1;
            }
            _rotatingToShoot = true;
        }
    }


    public override void Damage(Vector3 hitPosition)
    {
        bool justWood = Random.Range(0, 3) == 1 && _health > 60;

        if (justWood)
        {
            SpreadWoodWhenDamaged(hitPosition, _boat);
        }
        else
        {
            var piece = _boat.PickRandomPieceWhenDamaged();

            if(piece != null)
            {
                Canon canon = piece.GetComponent<Canon>();
                if (canon != null)
                {
                    _boat.CanonDestroyed(canon);
                }

                piece.Impulse();
                return;
            }
            SpreadWoodWhenDamaged(hitPosition, _boat);
        }
    }

    protected override void HadleHealth(int valueToAdd)
    {
        base.HadleHealth(valueToAdd);

        _boat.ChangeBoatAccordingToHealth((float)_health / 100);
    }

    public override void Die(bool playerKilled)
    {
        base.Die(playerKilled);
        _boat.DyingAnimation();
    }

}
