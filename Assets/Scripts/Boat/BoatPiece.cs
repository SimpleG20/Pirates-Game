using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoatPiece : MonoBehaviour
{
    private bool _destroyed;

    private List<BoatPiece> _boatPieces;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private CancellationTokenSource _tokenSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _boatPieces = new List<BoatPiece>();

        if (transform.childCount != 0)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var component = transform.GetChild(i).GetComponent<BoatPiece>();

                if (component != null) _boatPieces.Add(component);
            }
        }

        _tokenSource = new CancellationTokenSource();
    }

    private void Update()
    {
        if (_rigidbody == null) return;
        if (_rigidbody.velocity.magnitude <= 0) return;

        _rigidbody.AddForce(-_rigidbody.velocity * Random.Range(0.25f, 0.4f), ForceMode2D.Force);
    }

    public void FadeAnimation()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Fade");
    }

    public async void Impulse()
    {
        if (_destroyed) return;

        foreach (BoatPiece boatPiece in _boatPieces) boatPiece.Impulse();

        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
        }

        transform.parent = null;

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.AddForce(Random.insideUnitCircle * Random.Range(1.5f, 4f), ForceMode2D.Impulse);
        _rigidbody.AddTorque(4, ForceMode2D.Impulse);

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, _tokenSource.Token);
        if (_tokenSource.IsCancellationRequested) return;

        foreach(BoatPiece piece in _boatPieces) piece.FadeAnimation();

        FadeAnimation();
    }

    public void DestroyObject() { _destroyed = true;  Destroy(gameObject); }
}
