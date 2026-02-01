using System.Collections;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    public float delay = 1f;
    TrailRenderer trail;

    [SerializeField] private GameObject[] Particles;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void ActiveTrail()
    {
        StartCoroutine(DisableTrail());
        trail.emitting = true;
        foreach (var particle in Particles) {
            particle.GetComponent<ParticleSystem>().Play();

        }
    }

    private IEnumerator DisableTrail()
    {
        yield return new WaitForSeconds(delay);
        trail.emitting = false;
    }
}
