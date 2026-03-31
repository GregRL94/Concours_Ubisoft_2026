using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
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
        //UpdateGameTime();
    }

    //private void UpdateGameTime()
    //{
    //    if (GameManager.Instance != null)
    //    {
    //        float time = GameManager.Instance.CurrentTime;

    //        int minutes = (int)(time / 60);
    //        int seconds = (int)(time % 60);
    //        int ms = (int)((time * 100) % 100);
    //        string display = $"{minutes:D2}:{seconds:D2}:{ms:D2}";

    //        UpdateTimer(display);
    //    }
    //}

    #region TIMER

    public void UpdateTimer(string time)
    {
        print("update timer !");
        if (timerText != null)
            timerText.text = time;
    }

    #endregion

    #region OBJECTIVE
    public void UpdateMission(string mission)
    {
        if (missionText != null)
            missionText.text = mission;
    }
    public void UpdateObjective(string objective)
    {
        if (objectiveText != null)
            objectiveText.text = "|" + objective;
    }

    #endregion


    //public IEnumerator AnimateObjectiveText()
    //{
    //    Vector3 startScale = Vector3.zero;
    //    Vector3 targetScale = Vector3.one;

    //    float duration = 0.25f;
    //    float time = 0f;

    //    objectiveText.transform.localScale = startScale;

    //    // Scale up rapide
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = time / duration;
    //        objectiveText.transform.localScale = Vector3.Lerp(startScale, targetScale * 1.2f, t);
    //        yield return null;
    //    }

    //    // Petit bounce retour
    //    time = 0f;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = time / duration;
    //        objectiveText.transform.localScale = Vector3.Lerp(targetScale * 1.2f, targetScale, t);
    //        yield return null;
    //    }
    //}

}