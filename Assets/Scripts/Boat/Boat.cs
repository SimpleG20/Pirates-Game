using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Boat
{
    public BoatPiece WoodPrefab;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private List<BoatPiece> _bodyPieces;

    public BoatPiece PickRandomPieceWhenDamaged()
    {
        if (_bodyPieces.Count == 0) return null;

        var piece = _bodyPieces.PickRandomList(true);
        return piece;
    }

    public void ChangeBoatAccordingToHealth(float health)
    {
        _animator.SetFloat("Destruction", health);
    }

    public void ExplodeAnimation()
    {
        while (_bodyPieces.Count > 0)
        {
            var piece = PickRandomPieceWhenDamaged();
            if (piece != null) piece.Impulse();
        }

        if (_animator == null) return;
        _animator.SetTrigger("Explode");
    }

    public void DyingAnimation()
    {
        while(_bodyPieces.Count > 0)
        {
            var piece = PickRandomPieceWhenDamaged();
            if (piece != null) piece.Impulse();
        }

        if (_animator == null) return;
        _animator.SetTrigger("Die");
    }

}
