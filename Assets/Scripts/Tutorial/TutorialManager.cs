using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    private void Awake()
    {
        Instance = this;
    }

    #region Tutorial
    public void ShowTutorial(string message, float duration = 3f)
    {
        StartCoroutine(TutorialRoutine(message, duration));
    }

    IEnumerator TutorialRoutine(string message, float duration)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = message;

        yield return new WaitForSeconds(duration);

        tutorialPanel.SetActive(false);
    }

    #endregion
}