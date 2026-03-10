using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MechaUltimateUI : MonoBehaviour
{
    [Header("Ultimate UI")]
    [SerializeField] private Image ultimateFillImage;
    [SerializeField] private GameObject pulseUltimateImage;
    [SerializeField] private TextMeshProUGUI ultimateLabel;
    [Header("Hold Sync Ultimate UI")]
    [SerializeField] private GameObject holdUltimateImage;
    [SerializeField] private Image holdFillImage;
    [SerializeField] private TextMeshProUGUI holdUltimateLabel;

    [Header("Ready Animation")]
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float pulseScale = 0.15f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color readyColor;

    private float maxCharge;
    private bool isReady = false;

    private Vector3 baseScale;
    private Coroutine pulseCoroutine;

    public void Initialize(float maxValue)
    {
        holdUltimateImage?.SetActive(false);
        maxCharge = maxValue;

        if (ultimateFillImage != null)
        {
            ultimateFillImage.fillAmount = 0f;
            ultimateFillImage.color = normalColor;
            baseScale = ultimateFillImage.transform.localScale;
        }

        if (ultimateLabel != null)
            ultimateLabel.text = "ULTIMATE";

        isReady = false;
    }

    public void UpdateCharge(float currentCharge)
    {
        if (ultimateFillImage == null) return;

        float percent = Mathf.Clamp01(currentCharge / maxCharge);

        ultimateFillImage.fillAmount = percent;

        if (ultimateLabel != null)
        {
            if (percent >= 1f)
                ultimateLabel.text = "ULTIMATE READY";
            else
                ultimateLabel.text = $"ULTIMATE {(int)(percent * 100)}%";
        }

        // Ready state
        if (percent >= 1f && !isReady)
        {
            holdUltimateImage?.SetActive(true);
            isReady = true;

            if (ultimateFillImage != null)
                ultimateFillImage.color = readyColor;

            pulseCoroutine = StartCoroutine(PulseAnimation());
        }
    }

    private IEnumerator PulseAnimation()
    {
        float t = 0f;

        while (isReady)
        {
            t += Time.deltaTime * pulseSpeed;

            float scale = 1 + Mathf.Sin(t) * pulseScale;

            if (pulseUltimateImage != null)
                pulseUltimateImage.transform.localScale = baseScale * scale;

            yield return null;
        }
    }

    public void UpdateCoopHold(float percent)
    {
        if (!isReady) return;

        if (holdFillImage != null)
            holdFillImage.fillAmount = percent;

        if (holdUltimateLabel != null)
            holdUltimateLabel.text = $"SYNC {(int)(percent * 100)}%";
    }

    public void ResetUltimate()
    {
        holdUltimateImage?.SetActive(false);
        isReady = false;

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        if (ultimateFillImage != null)
        {
            ultimateFillImage.fillAmount = 0f;
            pulseUltimateImage.transform.localScale = baseScale;
            ultimateFillImage.color = normalColor;
        }

        if (ultimateLabel != null)
            ultimateLabel.text = "ULTIMATE";
    }
}