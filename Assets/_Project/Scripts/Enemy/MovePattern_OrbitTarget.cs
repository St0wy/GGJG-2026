using UnityEngine;

[CreateAssetMenu(menuName = "Enemies3D/Move Pattern/Orbit Target")]
public class MovePattern_OrbitTarget : EnemyMovePattern
{
    public float orbitSpeed = 3f;   // vitesse tangente
    public float approachSpeed = 1f; // pour garder une distance
    public float desiredRadius = 6f;

    public override Vector3 GetVelocity(float t, Transform enemy, Transform target)
    {
        if (target == null) return Vector3.zero;

        Vector3 offset = enemy.position - target.position;
        offset.y = 0f;

        if (offset.sqrMagnitude < 0.001f) offset = enemy.forward * desiredRadius;

        float dist = offset.magnitude;
        Vector3 radialDir = offset.normalized;

        // Tangente autour de la cible
        Vector3 tangent = Vector3.Cross(Vector3.up, radialDir).normalized;

        // Ajustement pour rester proche du radius voulu
        float radiusError = dist - desiredRadius;
        Vector3 correction = -radialDir * (radiusError * approachSpeed);

        return tangent * orbitSpeed + correction;
    }
}
