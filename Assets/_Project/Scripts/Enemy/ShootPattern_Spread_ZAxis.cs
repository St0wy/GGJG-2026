using UnityEngine;

[CreateAssetMenu(menuName = "Enemies3D/Shoot Pattern/Spread Z Axis")]
public class ShootPattern_Spread_ZAxis : EnemyShootPattern
{
    [Header("Spread")]
    public int bullets = 5;
    public float angleDegrees = 30f; // angle total horizontal
    public float muzzleSpeed = 18f;

    [Header("Direction")]
    public bool shootForward = true; // false = vers l'arrière

    public override void Fire(Transform firePoint, GameObject projectilePrefab, Transform target)
    {
        if (firePoint == null || projectilePrefab == null) return;

        int n = Mathf.Max(1, bullets);
        float half = angleDegrees * 0.5f;

        // Direction Z pure
        Vector3 baseDir = shootForward ? Vector3.forward : Vector3.back;

        for (int i = 0; i < n; i++)
        {
            float t = (n == 1) ? 0.5f : (float)i / (n - 1);
            float angle = Mathf.Lerp(-half, half, t);

            // Rotation UNIQUEMENT autour de Y (yaw)
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * baseDir;

            var go = Object.Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.LookRotation(dir, Vector3.up)
            );

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir.normalized * muzzleSpeed;
        }
    }
}
