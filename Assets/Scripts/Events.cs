using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static Action<float, Vector3> onHitPlayerBoat;
    public static void OnHitPlayerBoat(float distanceFactor, Vector3 hitPosition)
    {
        onHitPlayerBoat?.Invoke(distanceFactor, hitPosition);
    }
}
