using UnityEngine;

public abstract class EnemyMovePattern : ScriptableObject
{
    ///<summary>Returns the desired velocity (units/sec) according to time</summary>
    public abstract Vector3 GetVelocity(float t, Transform enemy, Transform target);
}
