using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ScreenFX : MonoBehaviour
{
    public static ScreenFX Instance;

    private GlobalVolumeManager volume;

    [Header("VIGNETTE SETTINGS")]
    [SerializeField] private float mainMenuVignetteIntensity = 0.2f;
    [SerializeField] private float gameplayVignetteIntensity = 0.4f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("DAMAGE FLASH")]
    [SerializeField] private float damageBoost = 0.3f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color damageColor = Color.red;
    private Coroutine damageRoutine;

    //[Header("DAMAGE FLASH")]
    //[SerializeField] private float flashPeak = 0.9f;
    //[SerializeField] private float flashInTime = 0.12f;
    //[SerializeField] private float flashOutTime = 0.25f;

    [Header("DASH")]
    [SerializeField] private float dashChromaticPeak = 0.7f;
    [SerializeField] private float dashDuration = 0.25f;

    [Header("ULTIMATE")]
    [SerializeField] private float bloomPeak = 6f;
    [SerializeField] private float ultimateDuration = 0.5f;

    [Header("LOW HP")]
    [SerializeField] private float lowHPHigh = 0.35f;
    [SerializeField] private float lowHPLow = 0.2f;
    [SerializeField] private float lowHPSpeed = 0.5f;

    private Coroutine lowHPCoroutine;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Abonnement sceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // On initialise volume maintenant pour Start
        volume = GlobalVolumeManager.Instance;
        ApplyFXForScene(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyFXForScene(scene);
    }

    private void ApplyFXForScene(Scene scene)
    {
        // Safe-check : on récupère le volume si null
        if (volume == null)
        {
            volume = GlobalVolumeManager.Instance;
            if (volume == null)
            {
                Debug.LogWarning("ScreenFX: GlobalVolumeManager not ready yet");
                return; // On attend la prochaine scène
            }
        }

        if (volume.Vignette == null)
        {
            Debug.LogWarning("ScreenFX: Vignette not found in GlobalVolumeManager");
            return;
        }

        // Applique l’intensité selon la scène
        if (scene.name == mainMenuSceneName)
        {
            volume.Vignette.intensity.value = mainMenuVignetteIntensity;
        }
        else
        {
            volume.Vignette.intensity.value = gameplayVignetteIntensity;
        }
    }

    // DAMAGE 
    public void TakeDamage()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damageRoutine = StartCoroutine(DamageRoutine()); // evite les update pr optimization
    }

    IEnumerator DamageRoutine()
    {
        var vignette = volume.Vignette;

        float elapsed = 0f;

        vignette.color.value = damageColor;
        vignette.intensity.value = gameplayVignetteIntensity + damageBoost;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / fadeDuration;

            vignette.color.value = Color.Lerp(damageColor, normalColor, t);
            vignette.intensity.value = Mathf.Lerp(gameplayVignetteIntensity + damageBoost, gameplayVignetteIntensity, t);

            yield return null;
        }

        vignette.color.value = normalColor;
        vignette.intensity.value = gameplayVignetteIntensity;

        damageRoutine = null;
    }

    // DAMAGE 2 FLASH
    //public void DamageFlash()
    //{
    //    StartCoroutine(DamageFlashRoutine());
    //}

    //IEnumerator DamageFlashRoutine()
    //{
    //    var vignette = volume.Vignette;

    //    float startIntensity = vignette.intensity.value;
    //    Color startColor = vignette.color.value;

    //    vignette.intensity.value = flashPeak;

    //    float t = 0;

    //    while (t < flashInTime)
    //    {
    //        t += Time.deltaTime;
    //        vignette.color.value = Color.Lerp(startColor, damageColor, t / flashInTime);
    //        yield return null;
    //    }

    //    t = 0;

    //    while (t < flashOutTime)
    //    {
    //        t += Time.deltaTime;

    //        vignette.intensity.value = Mathf.Lerp(flashPeak, startIntensity, t / flashOutTime);
    //        vignette.color.value = Color.Lerp(damageColor, startColor, t / flashOutTime);

    //        yield return null;
    //    }

    //    vignette.intensity.value = startIntensity;
    //    vignette.color.value = startColor;
    //}

    // DASH
    public void DashEffect()
    {
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        volume.Chromatic.intensity.value = dashChromaticPeak;

        yield return new WaitForSeconds(0.05f);

        float t = 0;

        while (t < dashDuration)
        {
            t += Time.deltaTime;
            volume.Chromatic.intensity.value =
                Mathf.Lerp(dashChromaticPeak, 0f, t / dashDuration);

            yield return null;
        }

        volume.Chromatic.intensity.value = 0;
    }

    // ULTIMATE
    public void UltimateEffect()
    {
        StartCoroutine(UltimateRoutine());
    }

    IEnumerator UltimateRoutine()
    {
        volume.Bloom.intensity.value = bloomPeak;

        yield return new WaitForSeconds(0.2f);

        float t = 0;

        while (t < ultimateDuration)
        {
            t += Time.deltaTime;

            volume.Bloom.intensity.value =
                Mathf.Lerp(bloomPeak, 1f, t / ultimateDuration);

            yield return null;
        }

        volume.Bloom.intensity.value = 1f;
    }

    // LOW HP PULSE
    public void StartLowHPPulse()
    {
        if (lowHPCoroutine != null) return;

        lowHPCoroutine = StartCoroutine(LowHPPulseRoutine());
    }

    IEnumerator LowHPPulseRoutine()
    {
        while (true)
        {
            volume.Vignette.intensity.value = lowHPHigh;
            yield return new WaitForSeconds(lowHPSpeed);

            volume.Vignette.intensity.value = lowHPLow;
            yield return new WaitForSeconds(lowHPSpeed);
        }
    }

    public void StopLowHPPulse()
    {
        if (lowHPCoroutine != null)
        {
            StopCoroutine(lowHPCoroutine);
            lowHPCoroutine = null;
        }

        volume.Vignette.intensity.value = gameplayVignetteIntensity;
    }
}