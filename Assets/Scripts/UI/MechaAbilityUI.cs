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
        public string abilityName;              // Nom de l'ability
        public Image abilityImage;              // Image à remplir pour le cooldown
        public TextMeshProUGUI abilityLabel;   // Label qui affiche le nom + timer
        [HideInInspector] public Coroutine currentCoroutine;
    }

    [Header("Configure your abilities in the Inspector")]
    public List<AbilityUI> abilities = new List<AbilityUI>();

    private void Start()
    {
        // Initialisation visuelle
        foreach (var ability in abilities)
        {
            if (ability.abilityImage != null) ability.abilityImage.fillAmount = 0f;
            if (ability.abilityLabel != null) ability.abilityLabel.text = $"{ability.abilityName}";
        }
    }

    // Déclenche le cooldown d'une ability en passant le cooldown en paramètre
    public void TriggerAbility(string abilityName, float cooldownDuration)
    {
        var ability = abilities.Find(a => a.abilityName == abilityName);
        if (ability != null)
        {
            // Stop la coroutine si elle existe déjà
            if (ability.currentCoroutine != null)
                StopCoroutine(ability.currentCoroutine);

            // Lancer la coroutine
            ability.currentCoroutine = StartCoroutine(CooldownRoutine(ability, cooldownDuration));
        }
        else
        {
            Debug.LogWarning($"Ability {abilityName} non trouvée dans UI !");
        }
    }

    private IEnumerator CooldownRoutine(AbilityUI ability, float cooldownDuration)
    {
        float timer = 0f;

        // Commence avec fillAmount à 0 pour signaler que l'ability vient d'être utilisée
        if (ability.abilityImage != null) ability.abilityImage.fillAmount = 1f;

        while (timer < cooldownDuration)
        {
            timer += Time.deltaTime;

            // FillAmount : 0 -> 1 sur toute la durée du cooldown
            if (ability.abilityImage != null)
                ability.abilityImage.fillAmount = Mathf.Clamp01(timer / cooldownDuration);

            // Texte : nom + secondes restantes
            if (ability.abilityLabel != null)
            {
                float remaining = Mathf.Ceil(cooldownDuration - timer);
                ability.abilityLabel.text = $"{ability.abilityName} {remaining}s";
            }

            yield return null;
        }

        // Cooldown terminé
        if (ability.abilityImage != null) ability.abilityImage.fillAmount = 0f;
        if (ability.abilityLabel != null) ability.abilityLabel.text = $"{ability.abilityName}";

        ability.currentCoroutine = null;
    }
}