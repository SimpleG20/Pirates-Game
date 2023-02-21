using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Boat
{
    public BoatPiece WoodPrefab;

    [Header("Transforms")]
    [SerializeField] private Transform _frontCanonTransform;
    [SerializeField] private Transform _lateralCanonTransform1;
    [SerializeField] private Transform _lateralCanonTransform2;
    [SerializeField] private Transform _lateralCanonTransform3;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private BoatPiece[] _bodyPieces;

    public Transform GetFrontCanon() => _frontCanonTransform;

    public Transform GetFirstLateralCanon() => _lateralCanonTransform1;

    public Transform GetSecondLateralCanon() => _lateralCanonTransform2;

    public Transform GetThirdLateralCanon() => _lateralCanonTransform3;

    public bool RemoveRandomPieceWhenDamaged()
    {
        if (_bodyPieces.Length == 0) return false;

        var piece = _bodyPieces.PickRandom(true);

        if (piece == null) return false;

        piece.Impulse();
        return true;
    }

    public void ChangeBoatAccordingToHealth(int health)
    {
        _animator.SetFloat("Destruction", health);
    }

    public void ExplodeAnimation()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Explode");
    }

    public void DyingAnimation()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Die");
    }

}
