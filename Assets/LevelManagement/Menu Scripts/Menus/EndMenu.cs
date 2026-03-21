using System.Collections;
using UnityEngine;

public class EndMenu : Menu
{
    [Header("References")]
    [SerializeField] private RectTransform titleText;
    [SerializeField] private GameObject buttonsGroup;

    [Header("Animation")]
    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float peakScale = 1.4f;
    [SerializeField] private float finalScale = 1f;

    [SerializeField] private float scaleUpDuration = 0.4f;
    [SerializeField] private float scaleDownDuration = 0.2f;
    [SerializeField] private float delayBeforeButtons = 0.3f;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 10f;

    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = titleText.position;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        titleText.position = initialPosition;
        titleText.localScale = Vector3.one * startScale;

        buttonsGroup.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(AnimationRoutine());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
    }

    IEnumerator AnimationRoutine()
    {
        yield return StartCoroutine(Scale(titleText, peakScale, scaleUpDuration));

        yield return StartCoroutine(Scale(titleText, finalScale, scaleDownDuration));

        yield return StartCoroutine(Shake());

        yield return new WaitForSecondsRealtime(delayBeforeButtons);

        buttonsGroup.SetActive(true);
    }

    // SCALE
    IEnumerator Scale(RectTransform target, float targetScale, float duration)
    {
        Vector3 start = target.localScale;
        Vector3 end = Vector3.one * targetScale;

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutBack(t / duration);

            target.localScale = Vector3.Lerp(start, end, lerp);

            yield return null;
        }

        target.localScale = end;
    }

    // SHAKE
    IEnumerator Shake()
    {
        float timer = 0f;
        Vector3 basePos = titleText.position;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            Vector3 offset = Random.insideUnitCircle * shakeStrength;
            titleText.position = basePos + offset;

            yield return null;
        }

        titleText.position = basePos;
    }

    // EASING
    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    // BUTTONS
    public void OnRestartPressed()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("UI_Submit");
        LevelLoader.ReloadLevel();
    }

    public void OnMainMenuPressed()
    {
        AudioManager.Instance.StopMusic();
        LevelLoader.LoadMainMenuLevel();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}