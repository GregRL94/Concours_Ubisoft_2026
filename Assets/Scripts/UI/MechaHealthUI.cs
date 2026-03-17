using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MechaHealthUI : MonoBehaviour
{
    [Header("UI HP")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform healthBarRoot;
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Chunk HP")]
    [SerializeField] private Image healthChunkFill;
    [SerializeField] private Color blinkStartColor = Color.gray;
    [SerializeField] private Color blinkColor = Color.white;

    [Header("Chunk Settings")]
    [SerializeField] private float chunkDelay = 0.25f;
    [SerializeField] private float chunkLerpDuration = 0.4f;
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkSpeed = 0.08f;

    [Header("Pulse")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float pulseScale = 0.05f;
    [SerializeField] private float pulseDuration = 0.1f;
    [SerializeField] private int lowHealthThreshold = 15;

    private float currentHealth;
    private float maxHealth;

    private Coroutine healthRoutine;
    private Coroutine chunkRoutine;
    private Coroutine pulseRoutine;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        currentHealth = maxHP;

        healthFill.fillAmount = 1f;
        healthFill.color = healthyColor;

        healthChunkFill.fillAmount = 1f;
        healthChunkFill.gameObject.SetActive(false);

        healthText.text = Mathf.RoundToInt(currentHealth) + "%";
        healthBarRoot.localScale = Vector3.one;
    }

    public void UpdateHealth(float newHealth)
    {
        float previousHealth = currentHealth;
        currentHealth = newHealth;

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        if (chunkRoutine != null)
            StopCoroutine(chunkRoutine);

        healthRoutine = StartCoroutine(LerpHealth(previousHealth, currentHealth));

        // Setup chunk bar
        healthChunkFill.gameObject.SetActive(true);
        healthChunkFill.fillAmount = previousHealth / maxHealth;
        healthChunkFill.color = blinkStartColor;

        chunkRoutine = StartCoroutine(AnimateChunk(previousHealth, currentHealth));

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

            float fillAmount = Mathf.Lerp(start / maxHealth, target / maxHealth, t);
            healthFill.fillAmount = fillAmount;

            float displayedHP = Mathf.Lerp(start, target, t);
            healthText.text = Mathf.RoundToInt(displayedHP) + "%";

            healthFill.color = (target <= lowHealthThreshold) ? lowHealthColor : healthyColor;

            yield return null;
        }

        healthFill.fillAmount = target / maxHealth;
        healthText.text = Mathf.RoundToInt(target) + "%";
        healthFill.color = (target <= lowHealthThreshold) ? lowHealthColor : healthyColor;
    }

    private IEnumerator AnimateChunk(float start, float target)
    {
        yield return new WaitForSeconds(chunkDelay);

        // Blink effect
        for (int i = 0; i < blinkCount; i++)
        {
            healthChunkFill.color = blinkColor;
            yield return new WaitForSeconds(blinkSpeed);

            healthChunkFill.color = blinkStartColor;
            yield return new WaitForSeconds(blinkSpeed);
        }

        float elapsed = 0f;

        float startFill = start / maxHealth;
        float targetFill = target / maxHealth;

        while (elapsed < chunkLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / chunkLerpDuration;

            healthChunkFill.fillAmount = Mathf.Lerp(startFill, targetFill, t);

            yield return null;
        }

        healthChunkFill.fillAmount = targetFill;

        healthChunkFill.gameObject.SetActive(false);
    }

    private IEnumerator PulseBar()
    {
        Vector3 currentScale = healthBarRoot.localScale;
        Vector3 targetScale = currentScale * (1 + pulseScale);

        float t = 0f;

        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            healthBarRoot.localScale = Vector3.Lerp(currentScale, targetScale, t / pulseDuration);
            yield return null;
        }

        t = 0f;
        currentScale = healthBarRoot.localScale;
        Vector3 originalScale = Vector3.one;

        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            healthBarRoot.localScale = Vector3.Lerp(currentScale, originalScale, t / pulseDuration);
            yield return null;
        }

        healthBarRoot.localScale = Vector3.one;
    }
}