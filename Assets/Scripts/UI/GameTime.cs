using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    private float timer = 0f;

    private string secondes;
    private string minutes;
    private string milliSecondes;

    void Update()
    {
        timer += Time.deltaTime;

        minutes = ((int)(timer / 60)).ToString("D2");
        secondes = ((int)(timer % 60)).ToString("D2");

        milliSecondes = ((int)((timer * 100) % 100)).ToString("D2");

        string display = minutes + ":" + secondes + ":" + milliSecondes;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateTimer(display);
    }

    public float GetFinalTime()
    {
        return timer;
    }
}