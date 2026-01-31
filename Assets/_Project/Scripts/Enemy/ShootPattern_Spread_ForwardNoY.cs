using UnityEngine;

[CreateAssetMenu(menuName = "Enemies3D/Shoot Pattern/Spread Forward No Y")]
public class ShootPattern_Spread_ForwardNoY : EnemyShootPattern
{
    [Header("Spread")]
    public int bullets = 5;
    public float angleDegrees = 30f;   // éventail gauche/droite
    public float muzzleSpeed = 18f;

    public override void Fire(Transform firePoint, GameObject projectilePrefab, Transform target)
    {
        if (firePoint == null || projectilePrefab == null) return;

        // 1) Base direction = forward local du firePoint, MAIS sans Y
        Vector3 baseDir = firePoint.forward;
        baseDir.y = 0f;

        // Si le firePoint regarde quasi verticalement, on fallback sur un axis safe
        if (baseDir.sqrMagnitude < 0.0001f)
            baseDir = firePoint.parent ? firePoint.parent.forward : Vector3.forward;


        baseDir.y = 0f;
        baseDir.Normalize();

        int n = Mathf.Max(1, bullets);
        float half = angleDegrees * 0.5f;

        for (int i = 0; i < n; i++)
        {
            float t = (n == 1) ? 0.5f : (float)i / (n - 1);
            float angle = Mathf.Lerp(-half, half, t);

            // 2) Spread uniquement autour de Y (yaw) => jamais de montée/descente
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;

            // Sécurité: on verrouille Y à 0, même après rotation
            dir.y = 0f;
            dir.Normalize();

            var go = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir, Vector3.up));

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * muzzleSpeed;
        }
    }
}
