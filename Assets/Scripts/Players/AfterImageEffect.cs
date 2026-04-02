using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer[] playerRenderers;
    [SerializeField] private SpriteRenderer[] afterImagePrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float timeBetweenImages = 0.05f;

    [Header("Lifetime Settings")]
    [SerializeField] private float imageLifetime = 0.3f;

    [Header("Dash Effect Settings")]
    [Tooltip("Pourcentage du dash pendant lequel l'effet est actif")]
    [Range(0f, 1f)]
    [SerializeField] private float activePercentOfDash = 0.8f;

    //[Header("Optional Override")]
    //[SerializeField] private bool useFixedImageCount = false;
    //[SerializeField] private int desiredImageCount = 4;

    [Header("Visual")]
    [SerializeField] private Color afterImageColor = new Color(1, 1, 1, 0.5f);

    private float spawnTimer;
    private float effectTimer;
    private bool isActive;

    private void Update()
    {
        if (!isActive) return;

        effectTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnAfterImage();
            spawnTimer = timeBetweenImages;
        }

        if (effectTimer <= 0f)
        {
            StopEffect();
        }
    }

    // Lance l'effet pour une duree base sur le dash
    //public void PlayDashEffect(float dashDuration)
    //{
    //    float activeDuration = dashDuration * activePercentOfDash;

    //    isActive = true;
    //    effectTimer = activeDuration;
    //    spawnTimer = 0f;
    //}

    public void PlayDashEffect(float dashDuration)
    {
        float activeDuration = dashDuration * activePercentOfDash;

        //if (useFixedImageCount && desiredImageCount > 0)
        //{
        //    timeBetweenImages = activeDuration / desiredImageCount;
        //}

        isActive = true;
        effectTimer = activeDuration;
        spawnTimer = 0f;
    }

    private void StopEffect()
    {
        isActive = false;
    }

    private void SpawnAfterImage()
    {
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] == null || afterImagePrefabs[i] == null) continue;

            SpriteRenderer image = Instantiate(afterImagePrefabs[i], transform.position, transform.rotation);

            image.sprite = playerRenderers[i].sprite;
            image.transform.localScale = transform.localScale;
            image.flipX = playerRenderers[i].flipX;
            image.color = afterImageColor;

            Destroy(image.gameObject, imageLifetime);
        }
    }
}