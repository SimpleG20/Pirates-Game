using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _whoShoot;
    private bool _destroying;

    [SerializeField] private float _bulletImpulse;
    [SerializeField] private float _bulletTime;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;

    private CancellationTokenSource _tokenSource;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _tokenSource = new CancellationTokenSource();

        yield return new WaitForSeconds(_bulletTime);
        if (gameObject != null) FadeBullet();
    }

    private void Update()
    {
        if (_rigidbody.velocity.magnitude <= 0) return;

        _rigidbody.AddForce(-_rigidbody.velocity.normalized * 0.2f, ForceMode2D.Force);
    }

    public void Instantited(int layer, Vector3 direction)
    {
        _whoShoot = layer;

        _rigidbody.AddForce(direction * _bulletImpulse, ForceMode2D.Impulse);
        //_rigidbody.velocity = direction * -_bulletSpeed;
        transform.parent = null;
    }

    private async void FadeBullet()
    {
        if (_destroying) return;

        _animator.SetTrigger("Fade");
        await Task.Delay(425);

        if (_tokenSource.IsCancellationRequested) return;
        Destroy(gameObject);
    }

    private async void DestroyBullet()
    {
        _rigidbody.velocity = Vector3.zero;
        _destroying = true;
        _animator.SetTrigger("Destroy");
        await Task.Delay(425);

        if (_tokenSource.IsCancellationRequested) return;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_whoShoot == collision.gameObject.layer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var damageFactor = (1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;
            //print(damageFactor);

            Events.OnHitPlayerBoat(damageFactor, transform.position);
            DestroyBullet();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            var damageFactor = (1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;
            //print(damageFactor);

            collision.gameObject.GetComponentInParent<Enemy>().GotHit(damageFactor, transform.position);
            DestroyBullet();
        }
        else if (collision.gameObject.CompareTag("Ground")) DestroyBullet();
    }
}
