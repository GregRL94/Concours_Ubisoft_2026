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
        public TextMeshProUGUI abilityLabel; // Txt nom + temps
        [HideInInspector] public Coroutine currentCoroutine;
    }

    [Header("Configure your abilities in the Inspector")]
    public List<AbilityUI> abilities = new List<AbilityUI>();

    [Header("Punch Animation Settings")]
    [SerializeField] private float punchDuration = 0.08f;
    [SerializeField] private float punchStrength = 0.15f;
    [SerializeField] private float punchFrequency = 1f; // optionnel pour modif la courbe


    private void Start()
    {
        foreach (var ability in abilities)
        {
            if (ability.abilityImage != null)
            {
                ability.abilityImage.fillAmount = 0f;
                ability.abilityImage.transform.localScale = Vector3.one;
            }

            if (ability.abilityLabel != null)
                ability.abilityLabel.text = ability.abilityName;
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

    private IEnumerator CooldownRoutine(AbilityUI ability, float cooldownDuration)
    {
        float timer = 0f;

        while (timer < cooldownDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / cooldownDuration);

            if (ability.abilityImage != null)
                ability.abilityImage.fillAmount = 1f - t;

            if (ability.abilityLabel != null)
            {
                float remaining = Mathf.Ceil(cooldownDuration - timer);
                ability.abilityLabel.text = $"{ability.abilityName} {remaining}s";
            }

            yield return null;
        }

        // Cooldown finished
        if (ability.abilityImage != null)
        {
            ability.abilityImage.fillAmount = 0f;
        }

        if (ability.abilityLabel != null)
            ability.abilityLabel.text = ability.abilityName;

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