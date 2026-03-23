using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelMenu : Menu
{
    //[SerializeField] private Text titleText;

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        //StopAllCoroutines();
    }

    // BUTTONS
    public void OnNextLevelPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        GameManager.Instance.LoadNextLevel();
    }

}