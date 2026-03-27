using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        if (GameManager.Instance != null)
        {
            float time = GameManager.Instance.CurrentTime;

            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int ms = (int)((time * 100) % 100);
            string display = $"{minutes:D2}:{seconds:D2}:{ms:D2}";

            UpdateTimer(display);
        }
    }

    #region TIMER

    public void UpdateTimer(string time)
    {
        print("update timer !");
        if (timerText != null)
            timerText.text = time;
    }

    #endregion

    #region OBJECTIVE

    public void UpdateObjective(string objective)
    {
        if (objectiveText != null)
            objectiveText.text = "|" + objective;
    }

    #endregion

    #region WIN/LOSE

    public void ShowWin()
    {

    }
    public void ShowLose()
    {

    }

    #endregion


}