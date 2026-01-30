using UnityEngine;

public abstract class EnemyShootPattern : ScriptableObject
{
    /// <summary>Called when the enemy must fire (you manage how many projectiles, directions, etc.)</summary>
    public abstract void Fire(Transform firePoint, GameObject projectilePrefab, Transform target);
}