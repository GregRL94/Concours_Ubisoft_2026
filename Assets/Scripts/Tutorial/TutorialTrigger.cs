using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Objects To Activate")]
    public GameObject[] objectsToActivate;

    [Header("Objects To Deactivate")]
    public GameObject[] objectsToDeactivate;

    [Header("Tutorial Message")]
    [TextArea]
    public string tutorialMessage;

    //[Header("Extra Events")]
    //public UnityEvent onTriggered;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            ExecuteTrigger();

            Destroy(gameObject);
        }
    }

    private void ExecuteTrigger()
    {
        //GameManager.Instance.MusicPlaylistStart();
        foreach (var obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (TutorialManager.Instance != null && !string.IsNullOrEmpty(tutorialMessage))
        {
            TutorialManager.Instance.ShowTutorial(tutorialMessage);
            //UIManager.Instance.UpdateObjective("Éliminer les aliens");
        }

        //onTriggered?.Invoke();
    }
}