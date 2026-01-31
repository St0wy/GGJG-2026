using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;

    private VisualElement fill;

    private void Start()
    {
        health = FindAnyObjectByType<PlayerController>().GetComponent<Health>();

        var root = GetComponent<UIDocument>().rootVisualElement;
        fill = root.Q<VisualElement>("HealthBarMask");
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        float ratio = (health.maxHealth <= 0) ? 0f : (float)health.currentHealth / health.maxHealth;
        float uv = 1f - ratio;
        fill.style.width = ratio * 250f;
    }
}
