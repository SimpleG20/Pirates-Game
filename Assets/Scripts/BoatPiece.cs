using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoatPiece : MonoBehaviour
{
    private List<BoatPiece> _boatPieces;

    private Rigidbody2D _rigidbody;
    private RelativeJoint2D _joint;
    private Animator _animator;


    private CancellationTokenSource _tokenSource;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _joint = GetComponent<RelativeJoint2D>();
        _animator = GetComponent<Animator>();

        _boatPieces = new List<BoatPiece>();

        if (transform.childCount != 0)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var component = transform.GetChild(i).GetComponent<BoatPiece>();

                if (component != null) _boatPieces.Add(component);
            }
        }

        print($"{gameObject.name} - {_boatPieces.Count}");
        _tokenSource = new CancellationTokenSource();
    }

    private void Update()
    {
        if (_rigidbody.velocity.magnitude <= 0) return;

        _rigidbody.AddForce(-_rigidbody.velocity * 0.3f, ForceMode2D.Force);
    }

    public void FadeAnimation()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Fade");
    }

    public void DeatchRelativeJoint()
    {
        if (_joint == null) return;

        _joint.connectedBody = null;
        
        if(_boatPieces.Count != 0)
        {
            foreach (BoatPiece piece in _boatPieces)
            {
                piece.DeatchRelativeJoint();
            }
        }
    }

    public async void Impulse()
    {
        DeatchRelativeJoint();

        _rigidbody.AddForce(Random.insideUnitCircle * Random.Range(2f, 4f), ForceMode2D.Impulse);
        _rigidbody.AddTorque(4, ForceMode2D.Impulse);

        await Task.Delay(1000);
        if (_tokenSource.IsCancellationRequested) return;

        foreach(BoatPiece piece in _boatPieces) 
        {
            piece.FadeAnimation();
        }

        FadeAnimation();
    }

    public void DestroyObject() => Destroy(gameObject);
}
