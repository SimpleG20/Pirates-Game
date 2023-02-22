using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _whoShot;
    private bool _destroying;

    [SerializeField] private float _bulletImpulse;
    [SerializeField] private float _bulletTime;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_bulletTime);
        if (gameObject != null) FadeBullet();
    }

    private void Update()
    {
        if (_rigidbody.velocity.magnitude <= 0) return;

        _rigidbody.AddForce(-_rigidbody.velocity.normalized * Random.Range(0.2f, 0.35f), ForceMode2D.Force);
    }

    public void Instantited(int layer, Vector3 direction)
    {
        _whoShot = layer;

        _rigidbody.AddForce(direction * Random.Range(_bulletImpulse * 0.9f, _bulletTime * 1.1f), ForceMode2D.Impulse);
        //_rigidbody.velocity = direction * -_bulletSpeed;
        transform.parent = null;
    }

    private void FadeBullet()
    {
        if (_destroying) return;

        _animator.SetTrigger("Fade");
    }

    private void DestroyBullet()
    {
        _rigidbody.velocity = Vector3.zero;
        _destroying = true;
        _animator.SetTrigger("Destroy");
    }

    public void DestroyObject() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_whoShot == collision.gameObject.layer) return;

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
