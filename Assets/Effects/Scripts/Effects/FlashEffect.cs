using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    //effet de flash quand quelque chose se prend des dégâts
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    //remplace le material du sprite par le material de flashMaterial pendant la durée de duration
    private IEnumerator FlashRoutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}

