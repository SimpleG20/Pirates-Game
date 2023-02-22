using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoat : MonoBehaviour
{
    [SerializeField] private float _secondOffsetMagnitude;
    [SerializeField] private Vector3 _offset;
    
    private float cos;

    private Transform _boat;
    private Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_boat == null) return;

        cos = Mathf.Abs(Mathf.Cos(_boat.localEulerAngles.z * Mathf.Deg2Rad));

        transform.position = cam.WorldToScreenPoint(_boat.position) +
            new Vector3(_offset.x, _offset.y + (cos * _secondOffsetMagnitude), _offset.z);
    }

    public void Initiate(Transform boat)
    {
        _boat = boat;
    }
}
