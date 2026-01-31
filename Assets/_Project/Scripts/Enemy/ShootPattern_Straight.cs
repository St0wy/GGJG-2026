using UnityEngine;

[CreateAssetMenu(menuName = "Enemies3D/Shoot Pattern/Straight")]
public class ShootPattern_Straight : EnemyShootPattern
{
    public float muzzleSpeed = 18f;

    public override void Fire(Transform firePoint, GameObject projectilePrefab, Transform target)
    {
        if (firePoint == null || projectilePrefab == null) return;

        Vector3 dir = firePoint.forward;
        if (target != null)
        {
            Vector3 toTarget = (target.position - firePoint.position);
            dir = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : firePoint.forward;
        }

        var go = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir, Vector3.up));
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * muzzleSpeed;
        if (go.TryGetComponent(out SecondProjectile proj)) proj.speed = muzzleSpeed;
    }
}
