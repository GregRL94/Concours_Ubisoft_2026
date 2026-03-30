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


    void Start()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterTrigger(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        if (!collision.CompareTag("Player")) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            ExecuteTrigger();

            //unregistre le trigger 
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.UnRegisterTrigger(this);
            }

            Destroy(gameObject);
        }
    }

    private void ExecuteTrigger()
    {
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
        }

        //onTriggered?.Invoke();
    }
}