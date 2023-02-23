using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    public int CanonDirection { get; private set; }

    private void Start()
    {
        if (gameObject.name.Contains("Left")) CanonDirection = 2;
        else if (gameObject.name.Contains("Right")) CanonDirection = 3;
        else CanonDirection = 1;
    }

    public bool IsPlayerWithinSight(float visionRange)
    {
        if (gameObject == null) return false;

        if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right), visionRange, _targetLayer))
        {
            return Random.Range(0,2) == 1;
        }
        else return false;
    }
}
