using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MechaHealthUI))]
public class MechaHealth : MonoBehaviour
{
    [SerializeField] private MechaHealthUI healthUI;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float iFrameDuration = 0.5f; // durée de l’invincibilité après un hit

    private float currentHealth;
    private bool isInIFrame = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.Initialize(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isInIFrame) return; // ignore damage si en iframe

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthUI.UpdateHealth(currentHealth);

        // Start i-frame
        StartCoroutine(StartIFrame());
    }

    private IEnumerator StartIFrame()
    {
        isInIFrame = true;
        yield return new WaitForSeconds(iFrameDuration);
        isInIFrame = false;
    }

    private void Update()
    {
        // DEBUG DAMAGE PLAYER
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            float damage = Random.Range(0, 15f);
            TakeDamage(damage);
        }
    }
}