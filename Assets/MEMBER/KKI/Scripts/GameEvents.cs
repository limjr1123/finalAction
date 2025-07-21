using System;

public static class GameEvents
{
    public static event Action<string> OnEnemyKilled;

    public static void EnemyKilled(string enemyId)
    {
        OnEnemyKilled?.Invoke(enemyId);
    }
}