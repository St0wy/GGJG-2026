using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Defaults")]
    public float defaultDuration = 0.15f;
    public float defaultMagnitude = 0.25f;

    private Vector3 originalLocalPos;
    private float shakeTime;
    private float shakeMagnitude;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        originalLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        originalLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (shakeTime > 0f)
        {
            Vector3 offset = Random.insideUnitSphere * shakeMagnitude;
            offset.z = 0f; // IMPORTANT : pas de shake en profondeur
            transform.localPosition = originalLocalPos + offset;

            shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalLocalPos;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeTime = Mathf.Max(shakeTime, duration);
        shakeMagnitude = magnitude;
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }
}
