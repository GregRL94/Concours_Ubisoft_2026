using System.Collections;
using TMPro;
using UnityEngine;

public class NextLevelMenu : Menu
{
    [Header("References")]
    [SerializeField] private RectTransform missionCompleteTitle;
    [SerializeField] private GameObject buttonsGroup;

    [Header("Positions")]
    [SerializeField] private RectTransform startPos;
    [SerializeField] private RectTransform centerPos;

    [Header("Timing")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float impactDuration = 0.5f;
    [SerializeField] private float delayBeforeButtons = 0.3f;

    [Header("Impact")]
    [SerializeField] private float impactScale = 1.3f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI missionTitleText;
    [SerializeField] private string missionCompleteFormat = "Mission {0} Complete";
    [SerializeField] private string[] missionNames;

    private Vector3 startInitial;

    private void Awake()
    {
        startInitial = startPos.position;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        missionCompleteTitle.position = startInitial;
        missionCompleteTitle.localScale = Vector3.one;

        buttonsGroup.SetActive(false);

        UpdateMissionText();

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
        AudioManager.Instance.PlaySound("SFX_LevelCompleted");
        // Slide 
        yield return StartCoroutine(Slide(missionCompleteTitle, centerPos.position, slideDuration));

        // Petit impact
        yield return StartCoroutine(Impact());

        // Petit délai
        yield return new WaitForSecondsRealtime(delayBeforeButtons);

        // Show boutons
        buttonsGroup.SetActive(true);
    }

    IEnumerator Slide(RectTransform target, Vector3 endPos, float duration)
    {
        Vector3 start = target.position;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = EaseOutCubic(t / duration);

            target.position = Vector3.Lerp(start, endPos, lerp);

            yield return null;
        }

        target.position = endPos;
    }

    IEnumerator Impact()
    {
        float t = 0f;

        while (t < impactDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / impactDuration;

            float scale = Mathf.Lerp(1f, impactScale, lerp);
            missionCompleteTitle.localScale = Vector3.one * scale;

            yield return null;
        }

        missionCompleteTitle.localScale = Vector3.one;
    }

    void UpdateMissionText()
    {
        int missionNumber = Mathf.Clamp(GameManager.Instance.CurrentObjectiveIndex, 1, 99);

        missionTitleText.text = string.Format(missionCompleteFormat, missionNumber);
    }


    float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    // BUTTONS
    public void OnNextLevelPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        GameManager.Instance.LoadNextLevel();
    }
}