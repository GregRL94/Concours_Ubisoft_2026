using System.Collections;
using TMPro;
using UnityEngine;

// seuils étoiles personnalisables
[System.Serializable]
public class StarThreshold
{
    public float maxTime;
}

public class EndMenu : Menu
{
    [Header("References")]
    [SerializeField] private RectTransform titleText;
    [SerializeField] private GameObject buttonsGroup;
    [SerializeField] private float delayBeforeBtns = 1f;

    [Header("Title Animation")]
    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float peakScale = 1.4f;
    [SerializeField] private float finalScale = 1f;
    [SerializeField] private float scaleUpDuration = 0.4f;
    [SerializeField] private float scaleDownDuration = 0.25f;

    [Header("Shake Animation")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeStrength = 12f;

    [Header("Stars Animation")]
    [SerializeField] private StarThreshold[] starThresholds;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private float delayBeforeStars = 0.5f;
    [SerializeField] private float delayBetweenStars = 0.3f;

    [Header("Final Time Animation")]
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private RectTransform finalTimeTransform;
    [SerializeField] private float punchStartScale = 0f;
    [SerializeField] private float punchPeakScale = 1.6f;
    [SerializeField] private float punchEndScale = 1f;
    [SerializeField] private float punchUpDuration = 0.25f;
    [SerializeField] private float punchDownDuration = 0.15f;
    [SerializeField] private float delayBeforePunch = 0.2f;
    [SerializeField] private float timerRevealDuration = 0.8f;

    [SerializeField] private FadeTransition restartTransition;
    [SerializeField] private FadeTransition mainMenuTransition;

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
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("SFX_WinGame");

        // titre punch + shake
        yield return StartCoroutine(ScalePunch(titleText));
        yield return StartCoroutine(Shake());

        float finalTime = GameManager.Instance.CurrentTime;

        // timer punch + reveal
        yield return StartCoroutine(PunchFinalTime());
        yield return StartCoroutine(AnimateFinalTime(finalTime));

        //// stars
        //int starsEarned = CalculateStars(finalTime);
        //yield return StartCoroutine(AnimateStars(starsEarned));

        yield return new WaitForSecondsRealtime(delayBeforeBtns);

        buttonsGroup.SetActive(true);
    }

    // SCALE TITLE
    IEnumerator ScalePunch(RectTransform target)
    {
        Vector3 start = Vector3.one * startScale;
        Vector3 peak = Vector3.one * peakScale;
        Vector3 end = Vector3.one * finalScale;

        float t = 0f;

        while (t < scaleUpDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutCubic(t / scaleUpDuration);
            target.localScale = Vector3.Lerp(start, peak, lerp);
            yield return null;
        }

        t = 0f;

        while (t < scaleDownDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseInOutQuad(t / scaleDownDuration);
            target.localScale = Vector3.Lerp(peak, end, lerp);
            yield return null;
        }

        target.localScale = end;
    }

    // SHAKE SMOOTH
    IEnumerator Shake()
    {
        float t = 0f;
        Vector3 basePos = initialPosition;

        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;

            float progress = t / shakeDuration;
            float damping = 1f - progress;

            float x = Mathf.Sin(t * 40f) * shakeStrength * damping;
            float y = Mathf.Cos(t * 35f) * shakeStrength * damping;

            titleText.position = basePos + new Vector3(x, y, 0f);

            yield return null;
        }

        titleText.position = basePos;
    }

    // TIMER PUNCH
    IEnumerator PunchFinalTime()
    {
        yield return new WaitForSecondsRealtime(delayBeforePunch);

        AudioManager.Instance.PlaySound("SFX_FinalTimer");

        finalTimeTransform.localScale = Vector3.one * punchStartScale;

        float t = 0f;

        while (t < punchUpDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutBack(t / punchUpDuration);

            finalTimeTransform.localScale = Vector3.Lerp(
                Vector3.one * punchStartScale,
                Vector3.one * punchPeakScale,
                lerp
            );

            yield return null;
        }

        t = 0f;

        while (t < punchDownDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / punchDownDuration;

            finalTimeTransform.localScale = Vector3.Lerp(
                Vector3.one * punchPeakScale,
                Vector3.one * punchEndScale,
                lerp
            );

            yield return null;
        }

        finalTimeTransform.localScale = Vector3.one * punchEndScale;
    }

    // TIMER ANIMATION
    IEnumerator AnimateFinalTime(float finalTime)
    {
        float t = 0f;

        while (t < timerRevealDuration)
        {
            t += Time.unscaledDeltaTime;

            float lerp = EaseOutCubic(t / timerRevealDuration);
            float currentTime = Mathf.Lerp(0f, finalTime, lerp);

            finalTimeText.text = FormatTime(currentTime);

            yield return null;
        }

        finalTimeText.text = FormatTime(finalTime);
    }

    // STARS
    int CalculateStars(float time)
    {
        int count = 0;

        for (int i = 0; i < starThresholds.Length; i++)
        {
            if (time <= starThresholds[i].maxTime)
                count++;
        }

        return count;
    }

    IEnumerator AnimateStars(int starCount)
    {
        yield return new WaitForSecondsRealtime(delayBeforeStars);

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                stars[i].SetActive(true);
                AudioManager.Instance.PlaySound("SFX_PopOutStar");
                StartCoroutine(StarPop(stars[i].transform));
            }

            yield return new WaitForSecondsRealtime(delayBetweenStars);
        }
    }

    IEnumerator StarPop(Transform star)
    {
        Vector3 start = Vector3.zero;
        Vector3 peak = Vector3.one * 1.5f;
        Vector3 end = Vector3.one;

        float t = 0f;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutBack(t / duration);

            star.localScale = Vector3.Lerp(start, peak, lerp);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseInOutQuad(t / duration);

            star.localScale = Vector3.Lerp(peak, end, lerp);
            yield return null;
        }

        star.localScale = end;
    }

    // FORMAT TIME
    string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int ms = (int)((time * 100) % 100);

        return $"{minutes:D2}:{seconds:D2}:{ms:D2}";
    }

    // EASING
    float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    float EaseInOutQuad(float x)
    {
        return x < 0.5f
            ? 2 * x * x
            : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    // BUTTONS
    public void OnRestartPressed()
    {
        TransitionManager.Instance.TransitionRestartScene(restartTransition);
    }

    public void OnMainMenuPressed()
    {
        MenuManager.Instance.CloseMenu();
        OnReturnToMainMenu();
        TransitionManager.Instance.TransitionToScene("MainMenu", mainMenuTransition, 0f);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}