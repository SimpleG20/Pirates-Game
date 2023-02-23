using System;
using UnityEngine;

public static class Events
{
    public static Action<float> onPlayerShoot;
    public static void OnPlayerShoot(float timeToShootAgain)
    {
        onPlayerShoot?.Invoke(timeToShootAgain);
    }

    public static Action<float, Vector3> onHitPlayerBoat;
    public static void OnHitPlayerBoat(float distanceFactor, Vector3 hitPosition)
    {
        onHitPlayerBoat?.Invoke(distanceFactor, hitPosition);
    }

    public static Action<int> onEnemyDefeated;
    public static void OnEnemyDefeated(int type)
    {
        onEnemyDefeated?.Invoke(type);
    }

    public static Action<bool> onPaused;
    public static void OnPaused(bool value)
    {
        onPaused?.Invoke(value);
    }

    public static Action onGameBeginning;
    public static void OnGameBeginning()
    {
        onGameBeginning?.Invoke();
    }

    public static Action onGameEnded;
    public static void OnGameEnded()
    {
        onGameEnded?.Invoke();
    }
}
