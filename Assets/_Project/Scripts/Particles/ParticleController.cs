using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }


}
