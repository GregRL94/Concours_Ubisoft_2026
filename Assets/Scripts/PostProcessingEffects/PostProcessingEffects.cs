using UnityEngine;
using System.Collections;

public class PostProcessingEffects : MonoBehaviour
{
    public static PostProcessingEffects Instance;

    GlobalVolumeManager volume;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    void Start()
    {
        volume = GlobalVolumeManager.Instance;
    }

    // DAMAGE FLASH
    public void DamageFlash()
    {
        StartCoroutine(DamageFlashRoutine());
    }

    IEnumerator DamageFlashRoutine()
    {
        var vignette = volume.Vignette;

        float startIntensity = vignette.intensity.value;

        Color startColor = vignette.color.value;
        Color red = new Color(0.8f, 0f, 0f);

        float peak = 0.9f;

        vignette.intensity.value = peak;

        float t = 0;

        while (t < 0.12f)
        {
            t += Time.deltaTime;

            vignette.color.value = Color.Lerp(startColor, red, t / 0.12f);

            yield return null;
        }

        t = 0;

        while (t < 0.25f)
        {
            t += Time.deltaTime;

            vignette.intensity.value = Mathf.Lerp(peak, startIntensity, t / 0.25f);
            vignette.color.value = Color.Lerp(red, startColor, t / 0.25f);

            yield return null;
        }

        vignette.intensity.value = startIntensity;
        vignette.color.value = startColor;
    }

    // DASH EFFECT
    public void DashEffect()
    {
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        float peak = 0.7f;

        volume.Chromatic.intensity.value = peak;

        yield return new WaitForSeconds(0.05f);

        float t = 0;

        while (t < 0.25f)
        {
            t += Time.deltaTime;
            volume.Chromatic.intensity.value =
                Mathf.Lerp(peak, 0f, t / 0.25f);

            yield return null;
        }

        volume.Chromatic.intensity.value = 0;
    }

    // ULTIMATE EFFECT
    public void UltimateEffect()
    {
        StartCoroutine(UltimateRoutine());
    }

    IEnumerator UltimateRoutine()
    {
        float bloomPeak = 6f;
        //float exposurePeak = 2f;

        volume.Bloom.intensity.value = bloomPeak;
        //volume.ColorAdjustments.postExposure.value = exposurePeak;

        yield return new WaitForSeconds(0.2f);

        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            volume.Bloom.intensity.value =
                Mathf.Lerp(bloomPeak, 1f, t / 0.5f);

            //volume.ColorAdjustments.postExposure.value =
            //    Mathf.Lerp(exposurePeak, 0f, t / 0.5f);

            yield return null;
        }

        volume.Bloom.intensity.value = 1f;
        //volume.ColorAdjustments.postExposure.value = 0;
    }

    // LOW HP PULSE
    public void StartLowHPPulse()
    {
        StartCoroutine(LowHPPulseRoutine());
    }

    IEnumerator LowHPPulseRoutine()
    {
        while (true)
        {
            volume.Vignette.intensity.value = 0.35f;
            yield return new WaitForSeconds(0.5f);

            volume.Vignette.intensity.value = 0.2f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StopLowHPPulse()
    {
        StopAllCoroutines();
        volume.Vignette.intensity.value = 0.2f;
    }
}