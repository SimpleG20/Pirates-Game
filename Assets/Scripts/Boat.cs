using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Boat
{
    public BoatPiece WoodPrefab;

    [Header("Transforms")]
    [SerializeField] private Transform _frontCanon;
    [SerializeField] private Transform _firstLeftCanon;
    [SerializeField] private Transform _secondLeftCanon;
    [SerializeField] private Transform _thirdLeftCanon;
    [SerializeField] private Transform _firstRightCanon;
    [SerializeField] private Transform _secondRightCanon;
    [SerializeField] private Transform _thirdRightCanon;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private List<BoatPiece> _bodyPieces;

    public Transform GetFrontCanon() => _frontCanon;

    public Transform GetFirstLeftCanon() => _firstLeftCanon;

    public Transform GetSecondLeftCanon() => _secondLeftCanon;

    public Transform GetThirdLeftCanon() => _thirdLeftCanon;

    public Transform GetFirstRightCanon() => _firstRightCanon;

    public Transform GetSecondRigthCanon() => _secondRightCanon;

    public Transform GetThirdRigthCanon() => _thirdRightCanon;

    public bool RemoveRandomPieceWhenDamaged()
    {
        if (_bodyPieces.Count == 0) return false;

        var piece = _bodyPieces.PickRandomList(true);
        piece.Impulse();

        //_bodyPieces = _bodyPieces.ResizeArray();
        return true;
    }

    public void ChangeBoatAccordingToHealth(float health)
    {
        _animator.SetFloat("Destruction", health);
    }

    public void ExplodeAnimation()
    {
        if (_animator == null) return;

        while (_bodyPieces.Count > 0)
        {
            RemoveRandomPieceWhenDamaged();
        }

        _animator.SetTrigger("Explode");
    }

    public void DyingAnimation()
    {
        if (_animator == null) return;

        while(_bodyPieces.Count > 0)
        {
            RemoveRandomPieceWhenDamaged();
        }

        _animator.SetTrigger("Die");
    }

}
