using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MechaAbilityUI : MonoBehaviour
{
    [System.Serializable]
    public class AbilityUI
    {
        public string abilityName;           // Nom ability
        public Image abilityImage;           // Image
        //public TextMeshProUGUI abilityLabel; // Txt nom + temps
        public Image cooldownErrorFlash; // Image rouge transparente pour l’effet red flag
        [HideInInspector] public Coroutine currentCoroutine;
        [HideInInspector] public Coroutine errorCoroutine; // pour éviter les overlaps
    }

    [Header("Configure your abilities in the Inspector")]
    public List<AbilityUI> abilities = new List<AbilityUI>();

    [Header("Punch Animation Settings")]
    [SerializeField] private float punchDuration = 0.08f;
    [SerializeField] private float punchStrength = 0.15f;
    [SerializeField] private float punchFrequency = 1f; // optionnel pour modif la courbe

    [Header("Cooldown Flag Settings")]
    private float cooldownRedFlagDuration = 0.15f;
    [SerializeField, Range(0f, 1f)]
    private float cooldownRedFlagMaxOpacity = 0.7f;
    private void Start()
    {
        foreach (var ability in abilities)
        {
            if (ability.abilityImage != null)
            {
                ability.abilityImage.fillAmount = 0f;
                ability.abilityImage.transform.localScale = Vector3.one;
            }

            //if (ability.abilityLabel != null)
            //    ability.abilityLabel.text = ability.abilityName;
        }
    }

    public void TriggerAbility(string abilityName, float cooldownDuration)
    {
        var ability = abilities.Find(a => a.abilityName == abilityName);

        if (ability != null)
        {
            if (ability.currentCoroutine != null)
                StopCoroutine(ability.currentCoroutine);

            if (ability.abilityImage != null)
            {
                // Instant feedback
                ability.abilityImage.fillAmount = 1f;

                // Punch animation
                StartCoroutine(PunchAnimation(ability.abilityImage.transform));
            }

            ability.currentCoroutine = StartCoroutine(CooldownRoutine(ability, cooldownDuration));
        }
        else
        {
            Debug.LogWarning($"Ability {abilityName} not found in UI!");
        }
    }

    public void TriggerAbilityCooldownRedFlag(string abilityName)
    {
        var ability = abilities.Find(a => a.abilityName == abilityName);
        if (ability == null || ability.cooldownErrorFlash == null) return;

        // Stopper animation précédente 
        if (ability.errorCoroutine != null)
            StopCoroutine(ability.errorCoroutine);

        ability.errorCoroutine = StartCoroutine(CooldownRedFlagCoroutine(ability));
    }

    private IEnumerator CooldownRedFlagCoroutine(AbilityUI ability)
    {
        Color baseColor = ability.cooldownErrorFlash.color;
        float t = 0f;

        // Fade in
        while (t < cooldownRedFlagDuration * 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, cooldownRedFlagMaxOpacity, t / (cooldownRedFlagDuration * 0.5f));
            ability.cooldownErrorFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        t = 0f;

        // Fade out
        while (t < cooldownRedFlagDuration * 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(cooldownRedFlagMaxOpacity, 0f, t / (cooldownRedFlagDuration * 0.5f));
            ability.cooldownErrorFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        // Reset 
        ability.cooldownErrorFlash.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        ability.errorCoroutine = null;
    }

    private IEnumerator CooldownRoutine(AbilityUI ability, float cooldownDuration)
    {
        float timer = 0f;

        while (timer < cooldownDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / cooldownDuration);

            if (ability.abilityImage != null)
                ability.abilityImage.fillAmount = 1f - t;

            //if (ability.abilityLabel != null)
            //{
            //    float remaining = Mathf.Ceil(cooldownDuration - timer);
            //    ability.abilityLabel.text = $"{ability.abilityName} {remaining}s";
            //}

            yield return null;
        }

        // Cooldown finished
        if (ability.abilityImage != null)
        {
            ability.abilityImage.fillAmount = 0f;
        }

        //if (ability.abilityLabel != null)
        //    ability.abilityLabel.text = ability.abilityName;

        ability.currentCoroutine = null;
    }

    private IEnumerator PunchAnimation(Transform target)
    {
        float t = 0f;
        Vector3 originalScale = Vector3.one;

        while (t < punchDuration)
        {
            t += Time.deltaTime;

            float normalizedTime = t / punchDuration;

            float punch = 1f + Mathf.Sin(normalizedTime * Mathf.PI * punchFrequency) * punchStrength;

            target.localScale = originalScale * punch;

            yield return null;
        }

        target.localScale = originalScale;
    }
}