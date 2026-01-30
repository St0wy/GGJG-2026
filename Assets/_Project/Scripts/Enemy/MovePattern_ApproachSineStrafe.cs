using UnityEngine;

[CreateAssetMenu(menuName = "Enemies3D/Move Pattern/Approach + Sine Strafe")]
public class MovePattern_ApproachSineStrafe : EnemyMovePattern
{
    [Header("Approach")]
    public float forwardSpeed = 3f;

    [Header("Strafe (side)")]
    public float strafeAmplitude = 2f;
    public float strafeFrequency = 1.2f;

    public override Vector3 GetVelocity(float t, Transform enemy, Transform target)
    {
        if (target == null) return Vector3.zero;

        Vector3 toTarget = target.position - enemy.position;
        toTarget.y = 0f;
        Vector3 forward = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : enemy.forward;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        float strafe = Mathf.Sin(t * strafeFrequency * Mathf.PI * 2f) * strafeAmplitude;

        Vector3 v = forward * forwardSpeed + right * strafe;
        return v;
    }
}
