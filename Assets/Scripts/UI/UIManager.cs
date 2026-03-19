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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region TIMER

    public void UpdateTimer(string time)
    {
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