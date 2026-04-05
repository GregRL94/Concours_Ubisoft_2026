using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameOverMenu : Menu
{
    [Header("References")]
    [SerializeField] private RectTransform gameText;
    [SerializeField] private RectTransform overText;
    [SerializeField] private GameObject buttonsGroup;

    [Header("Positions (Transforms to drag)")]
    [SerializeField] private RectTransform gameStart;
    [SerializeField] private RectTransform gameCenter;

    [SerializeField] private RectTransform overStart;
    [SerializeField] private RectTransform overCenter;

    [Header("Timing")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float delayBetweenTexts = 0.15f;
    [SerializeField] private float delayShowBtn = 0.5f;
    [SerializeField] private float impactDuration = 0.15f;
    [SerializeField] private float glitchDuration = 0.25f;

    [Header("Impact")]
    [SerializeField] private float impactScale = 1.3f;
    [SerializeField] private float shakeStrength = 15f;

    [SerializeField] private FadeTransition restartTransition;
    [SerializeField] private FadeTransition mainMenuTransition;

    private Vector3 gameStartInitial;
    private Vector3 overStartInitial;


    private void Awake()
    {
        gameStartInitial = gameStart.position;
        overStartInitial = overStart.position;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        gameText.position = gameStartInitial;
        overText.position = overStartInitial;

        gameText.localScale = Vector3.one;
        overText.localScale = Vector3.one;

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
        AudioManager.Instance.PlaySound("SFX_GameOver");

        yield return StartCoroutine(Slide(gameText, gameCenter.position, slideDuration));

        yield return new WaitForSecondsRealtime(delayBetweenTexts);

        yield return StartCoroutine(Slide(overText, overCenter.position, slideDuration));

        yield return StartCoroutine(Impact());

        yield return StartCoroutine(Glitch());

        yield return new WaitForSecondsRealtime(delayShowBtn);

        buttonsGroup.SetActive(true);
    }

    // ---------------------------
    // SLIDE
    // ---------------------------
    IEnumerator Slide(RectTransform target, Vector3 endPos, float duration)
    {
        Vector3 start = target.position;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutExpo(t / duration);

            target.position = Vector3.Lerp(start, endPos, lerp);

            yield return null;
        }

        target.position = endPos;
    }

    // ---------------------------
    // IMPACT
    // ---------------------------
    IEnumerator Impact()
    {
        float t = 0f;

        while (t < impactDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / impactDuration;

            float scale = Mathf.Lerp(1f, impactScale, lerp);

            gameText.localScale = Vector3.one * scale;
            overText.localScale = Vector3.one * scale;

            yield return null;
        }

        // retour à 1
        gameText.localScale = Vector3.one;
        overText.localScale = Vector3.one;

        // SHAKE
        float shakeTime = 0.2f;
        float timer = 0f;

        Vector3 gameBase = gameText.position;
        Vector3 overBase = overText.position;

        while (timer < shakeTime)
        {
            timer += Time.unscaledDeltaTime;

            Vector3 offset = Random.insideUnitCircle * shakeStrength;

            gameText.position = gameBase + offset;
            overText.position = overBase - offset;

            yield return null;
        }

        gameText.position = gameBase;
        overText.position = overBase;
    }

    // ---------------------------
    // GLITCH
    // ---------------------------
    IEnumerator Glitch()
    {
        float timer = 0f;

        Vector3 gameBase = gameText.position;
        Vector3 overBase = overText.position;

        while (timer < glitchDuration)
        {
            timer += Time.unscaledDeltaTime;

            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-5f, 5f);

            gameText.position = gameBase + new Vector3(x, y, 0);
            overText.position = overBase - new Vector3(x, y, 0);

            yield return new WaitForSecondsRealtime(0.02f);

            gameText.position = gameBase;
            overText.position = overBase;
        }
    }

    // ---------------------------
    // EASING
    // ---------------------------
    float EaseOutExpo(float x)
    {
        return x >= 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }

    // ---------------------------
    // BUTTONS
    // ---------------------------
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