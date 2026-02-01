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
        foreach (var particle in Particles) {
            particle.SetActive(true);
        }
    }

    private IEnumerator DisableTrail()
    {
        yield return new WaitForSeconds(delay);
        trail.enabled = false;
        foreach (var particle in Particles)
        {
            particle.SetActive(false);
        }
    }
}
