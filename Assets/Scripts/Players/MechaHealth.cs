using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MechaHealthUI))]
public class MechaHealth : MonoBehaviour
{
    [Header("Mecha HP Parameters")]
    [SerializeField] private MechaHealthUI healthUI;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameObject _onHitEffect;
    [SerializeField] private float iFrameDuration = 0.5f; // durée de l’invincibilité après un hit
    
    [Header("Mecha Destroyed Parameters")]
    //[SerializeField] private float timeBeforeDisapear = 1f;
    [SerializeField] private float timeAnimDeath = 1f;
    [SerializeField] private float delayBeforeGameOverScreen = 1f;
    [SerializeField] private GameObject explosiveFX; 
    [SerializeField] private GameObject mecha_Base; 
    [SerializeField] private GameObject mecha_Top; 

    private float currentHealth;
    private bool isInIFrame = false;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.Initialize(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isInIFrame) return; // ignore damage si en iframe

        if(AccessibilityManager.Instance != null)
            damage = AccessibilityManager.Instance.ModifyEnemyDamageDealt(damage);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        ScreenFX.Instance.TakeDamage();
        _onHitEffect.GetComponent<ParticleSystem>().Play();
        mecha_Base.GetComponent<FlashEffect>().Flash();
        mecha_Top.GetComponent<FlashEffect>().Flash();
        healthUI.UpdateHealth(currentHealth);

        // Start i-frame
        StartCoroutine(StartIFrame());

        // Death Condition
        if (isDead) return;
        if (currentHealth <= 0)
        { 
            isDead = true;
            StartCoroutine(DeathRoutine());
        }
    }


    IEnumerator DeathRoutine()
    {
        // Disable controls
        //InputManager.Instance.DisableAll();
        PlayerInputHandler[] activeObjects = FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None);
        foreach (var item in activeObjects)
        {
            item.enabled = false;
        }

        mecha_Base.GetComponent<Animator>().SetTrigger("isDead");
        mecha_Top.GetComponent<Animator>().SetTrigger("isDead");
        yield return new WaitForSeconds(timeAnimDeath);

        // Explosion Effect
        //Instantiate(explosiveFX, transform.position, Quaternion.identity);

        // Camera shake
        if (mecha_Base.TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }

        // Hide mech mesh
        //Invoke("MechaDestroyed", timeBeforeDisapear);

        // Delay before GameoverScreen
        yield return new WaitForSeconds(delayBeforeGameOverScreen);


        GameManager.Instance.LoseGame();
    }

    void MechaDestroyed()
    {
        mecha_Base.SetActive(false);
        mecha_Top.SetActive(false); 
    }

    private IEnumerator StartIFrame()
    {
        isInIFrame = true;
        yield return new WaitForSeconds(iFrameDuration);
        isInIFrame = false;
    }

    private void Update()
    {
        // todo: DEBUG DAMAGE PLAYER
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame && !isDead)
        {
            float damage = Random.Range(0, 30f);
            TakeDamage(100);

        }
    }
}