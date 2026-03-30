using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private float displayDuration = 3;

    [Header("Position")]
    [SerializeField] private float hiddenOffsetY = 400f; // distance vers le bas

    [Header("Mecha Feel")]
    [SerializeField] private float overshoot = 15f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform panelRect;

    private Vector2 endPos;
    private Vector2 startPos;

    private void Awake()
    {
        Instance = this;
        panelRect = tutorialPanel.GetComponent<RectTransform>();

        // Position finale = celle dans ton UI
        endPos = panelRect.anchoredPosition;

        // Position de départ = en bas
        startPos = endPos + Vector2.down * hiddenOffsetY;
    }

    public void ShowTutorial(string message)
    {
        StopAllCoroutines();
        StartCoroutine(Routine(message));
    }

    IEnumerator Routine(string message)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = message;

        // reset
        panelRect.anchoredPosition = startPos;

        yield return Move(startPos, endPos, true);

        yield return new WaitForSeconds(displayDuration);

        yield return Move(endPos, startPos, false);

        tutorialPanel.SetActive(false);
    }

    IEnumerator Move(Vector2 from, Vector2 to, bool useOvershoot)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            float eval = curve.Evaluate(t);

            Vector2 pos = Vector2.LerpUnclamped(from, to, eval);

            // petit effet mecha propre
            if (useOvershoot && t > 0.85f)
            {
                float bump = Mathf.Sin((t - 0.85f) * 20f) * overshoot * (1f - t);
                pos.y += bump;
            }

            panelRect.anchoredPosition = pos;
            yield return null;
        }

        panelRect.anchoredPosition = to;
    }
}