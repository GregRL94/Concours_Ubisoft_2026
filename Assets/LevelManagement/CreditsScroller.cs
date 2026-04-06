using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    [Header("Configuration du scroll")]
    public float scrollSpeed = 50f;
    public RectTransform creditsText;
    public float endOffset = 100f; //Distance après laquelle le texte fini

    private Vector2 startPosition;
    private bool isScrolling = false;

    private void OnEnable()
    {
        if (creditsText == null)
        {
            Debug.LogError("CreditsScroller: Aucun RectTransform assigné !");
            enabled = false;
            return;
        }

        // sauvegarde la position de départ
        startPosition = creditsText.anchoredPosition;
        isScrolling = true;
    }

    private void Update()
    {
        if (!isScrolling) return;

        // Déplacement vers le haut
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // On vérifie si on a dépassé la limite
        if (creditsText.anchoredPosition.y >= startPosition.y + creditsText.rect.height + endOffset)
        {
            isScrolling = false;
            OnCreditsFinished();
        }
    }

    private void OnCreditsFinished()
    {
        gameObject.SetActive(false);
        Debug.Log("Crédits terminés !");
    }
}