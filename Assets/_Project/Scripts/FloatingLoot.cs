using UnityEngine;

public class FloatinngLoot : MonoBehaviour
{
    [Header("Float")]
    public float amplitude = 0.25f;     // hauteur de va-et-vient
    public float frequency = 1.5f;      // vitesse d'oscillation
    public bool useUnscaledTime = false;

    [Header("Rotate")]
    public bool rotate = true;
    public float rotateSpeed = 90f;     // degrés/sec
    public Vector3 rotateAxis = Vector3.up;

    [Header("Optional")]
    public float bobStartRandomOffset = 1f;

    private Vector3 startPos;
    private float phase;

    private void Awake()
    {
        startPos = transform.position;
        phase = Random.Range(0f, bobStartRandomOffset);
    }


    private void Update()
    {
        float t = (useUnscaledTime ? Time.unscaledTime : Time.time) + phase;

        float y = Mathf.Sin(t * frequency * Mathf.PI * 2f) * amplitude;
        transform.position = startPos + Vector3.up * y;

        if (rotate)
            transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
