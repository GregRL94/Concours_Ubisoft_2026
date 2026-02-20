using DG.Tweening;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FadeTransition : MonoBehaviour, ITransition
{
    private CanvasGroup canvasGroup;
    [SerializeField] private TransitionConfigSO config;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // preservation between scenes 

        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
    }

    public Tween PlayIn()
    {
        return canvasGroup
            .DOFade(1f, config.duration)
            .SetEase(config.ease);
    }

    public Tween PlayOut()
    {
        return canvasGroup
            .DOFade(0f, config.duration)
            .SetEase(config.ease)
            .OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                Destroy(gameObject); // destroys itself at the end of transition
            });
    }
}
