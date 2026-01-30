using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleProjectile : MonoBehaviour
{
    public float lifeTime = 6f;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}
