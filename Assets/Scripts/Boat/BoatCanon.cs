using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoatCanon : Boat
{
    [Header("Transforms")]
    [SerializeField] private Transform _frontCanon;
    [SerializeField] private Transform _firstLeftCanon;
    [SerializeField] private Transform _secondLeftCanon;
    [SerializeField] private Transform _thirdLeftCanon;
    [SerializeField] private Transform _firstRightCanon;
    [SerializeField] private Transform _secondRightCanon;
    [SerializeField] private Transform _thirdRightCanon;

    [SerializeField] private List<Canon> _canons;

    public List<Canon> GetCanons() => _canons;

    public Transform GetFrontCanon() => _frontCanon;

    public Transform GetFirstLeftCanon() => _firstLeftCanon;

    public Transform GetSecondLeftCanon() => _secondLeftCanon;

    public Transform GetThirdLeftCanon() => _thirdLeftCanon;

    public Transform GetFirstRightCanon() => _firstRightCanon;

    public Transform GetSecondRightCanon() => _secondRightCanon;

    public Transform GetThirdRightCanon() => _thirdRightCanon;

    public void CanonDestroyed(Canon canon)
    {
        _canons.Remove(canon);
    }
}
