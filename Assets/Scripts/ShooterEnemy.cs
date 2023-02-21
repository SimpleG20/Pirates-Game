using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    [Header("Shooter Variables")]
    [SerializeField] private int _cooldownFrontalShoot;
    [SerializeField] private int _cooldownLateralShoot;

    [SerializeField] private Bullet _bulletPrefab;
    
    private bool _ableToShoot;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Shoot();
        }
    }

    public override void Move()
    {
        
    }

    public async void Shoot()
    {
        var canons = new Transform[] { Boat.GetFirstLateralCanon(), Boat.GetSecondLateralCanon(), Boat.GetThirdLateralCanon() };

        for (int i = 0; i < canons.Length; i++)
        {
            if (canons[i] == null) continue;

            var bullet = Instantiate(_bulletPrefab, canons[i].position, Quaternion.identity);
            bullet.Instantited(_enemyLayer, canons[i].forward);
        }
        _ableToShoot = false;

        await Task.Delay(1000 * _cooldownLateralShoot);
        if (TokenSource.IsCancellationRequested) return;

        _ableToShoot = true;
    }

    public override async void Die()
    {
        Boat.DyingAnimation();
        await Task.Delay(2250);

        if (TokenSource.IsCancellationRequested) return;
        Destroy(gameObject);
    }
}
