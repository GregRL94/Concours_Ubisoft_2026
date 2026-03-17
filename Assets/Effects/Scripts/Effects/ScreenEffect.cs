using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffect : MonoBehaviour
{
    //Effets visuels à l'écran avec le Global Volume
    static ScreenEffect _instance; //Instance de la classe
    public static ScreenEffect instance => _instance; //Permet d'accéder à l'instance de la classe
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float maxIntensity = 0.5f;
    [SerializeField] private float damageBoost = 0.3f;
    private Vignette vignette;

    [SerializeField] private Color normalColor = Color.black; // Couleur par défaut de la vignette
    [SerializeField] private Color damageColor = Color.red; // Couleur de la vignette lorsqu'un dégât est pris
    [SerializeField] private float fadeDuration = 1f; // Durée pour revenir à la couleur normale

    private float damageTime;
    private bool isTakingDamage = false;

    void Start()
    {
        if (_instance == null) _instance = this;

        postProcessVolume.profile.TryGet(out vignette);
        vignette.color.value = normalColor;
    }

    void Update()
    {
        if (isTakingDamage)
        {
            float t = (Time.time - damageTime) / fadeDuration;
            vignette.color.value = Color.Lerp(damageColor, normalColor, t);
            vignette.intensity.value = Mathf.Lerp(maxIntensity + damageBoost, maxIntensity, t);
            // vignette.color.overrideState = true; // Assurer que la modification est appliquée

            if (t >= 1f)
                isTakingDamage = false;
        }
    }

    public void TakeDamage() //effet de vignette rouge de dégat
    {
        damageTime = Time.time;
        isTakingDamage = true;
        vignette.color.value = damageColor;
    }
}

