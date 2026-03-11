using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MechaHealthUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform healthBarRoot;

    [Header("Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Settings")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float pulseScale = 0.05f;
    [SerializeField] private float pulseDuration = 0.1f;
    [SerializeField] private int lowHealthThreshold = 15;

    private float currentHealth;
    private float maxHealth;

    private Coroutine healthRoutine;
    private Coroutine pulseRoutine;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        currentHealth = maxHP;

        healthFill.fillAmount = 1f;
        healthFill.color = healthyColor;

        healthText.text = Mathf.RoundToInt(currentHealth) + "%";
        healthBarRoot.localScale = Vector3.one;
    }

    public void UpdateHealth(float newHealth)
    {
        float previousHealth = currentHealth;
        currentHealth = newHealth;

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        healthRoutine = StartCoroutine(LerpHealth(previousHealth, currentHealth));

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);


        pulseRoutine = StartCoroutine(PulseBar());
    }

    private IEnumerator LerpHealth(float start, float target)
    {
        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lerpDuration;

            // Lerp fill
            float fillAmount = Mathf.Lerp(start / maxHealth, target / maxHealth, t);
            healthFill.fillAmount = fillAmount;

            // Lerp text
            float displayedHP = Mathf.Lerp(start, target, t);
            healthText.text = Mathf.RoundToInt(displayedHP) + "%";

            // Update color if low health
            healthFill.color = (target <= lowHealthThreshold) ? lowHealthColor : healthyColor;

            yield return null;
        }

        // Ensure final values
        healthFill.fillAmount = target / maxHealth;
        healthText.text = Mathf.RoundToInt(target) + "%";
        healthFill.color = (target <= lowHealthThreshold) ? lowHealthColor : healthyColor;
    }

private IEnumerator PulseBar()
{
    // Toujours partir de la scale actuelle
    Vector3 currentScale = healthBarRoot.localScale;
    Vector3 targetScale = currentScale * (1 + pulseScale);

    float t = 0f;

    // Pulse up
    while (t < pulseDuration)
    {
        t += Time.deltaTime;
        healthBarRoot.localScale = Vector3.Lerp(currentScale, targetScale, t / pulseDuration);
        yield return null;
    }

    t = 0f;
    currentScale = healthBarRoot.localScale; // prend la scale actuelle au moment du retour
    Vector3 originalScale = Vector3.one;

    // Pulse back vers scale normal (Vector3.one)
    while (t < pulseDuration)
    {
        t += Time.deltaTime;
        healthBarRoot.localScale = Vector3.Lerp(currentScale, originalScale, t / pulseDuration);
        yield return null;
    }

    // Assure que la scale finale est exactement la bonne
    healthBarRoot.localScale = Vector3.one;
}
}