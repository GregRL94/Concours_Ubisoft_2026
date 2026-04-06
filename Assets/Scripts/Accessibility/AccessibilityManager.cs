using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance;

    [Header("Damage Multipliers")]

    [Tooltip("Damage DEALT by the player to enemies (1 = default, >1 = stronger damage)")]
    [Range(0.5f, 2f)] public float playerDamageDealtMultiplier = 1f;

    [Tooltip("Damage DEALT by enemies to the player (1 = normal, >1 = stronger damage)")]
    [Range(0.5f, 2f)] public float enemyDamageDealtMultiplier = 1f;

    public bool crtEffectsEnabled = true; // par defaut avec post processing
    public bool aimReticuleMode = false; // par defaut sans reticule

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetCRTEffects(crtEffectsEnabled);
    }

    // DAMAGE MODIFIERS
    // Modify damage dealt by the player (to enemies)
    public float ModifyPlayerDamageDealt(float damage)
    {
        //float customDamage = damage * playerDamageDealtMultiplier;
        return damage * playerDamageDealtMultiplier;
    }

    // Modify damage dealt by enemies (to player)
    public float ModifyEnemyDamageDealt(float damage)
    {
        //float customDamage = damage * enemyDamageDealtMultiplier;
        return damage * enemyDamageDealtMultiplier;
    }

    // CRT SCREEN EFFECTS
    public void SetCRTEffects(bool enabled)
    {
        crtEffectsEnabled = enabled;

        var gvm = GlobalVolumeManager.Instance;

        if (gvm == null)
        {
            Debug.LogWarning("GlobalVolumeManager not found!");
            return;
        }

        SetEffectState(gvm.FilmGrain, enabled);
        SetEffectState(gvm.LensDistortion, enabled);
        SetEffectState(gvm.PaniniProjection, enabled);
        SetEffectState(gvm.Chromatic, enabled);
    }

    void SetEffectState(VolumeComponent effect, bool enabled)
    {
        if (effect == null) return;

        effect.active = true; // toujours actif côté volume pr performance
        effect.SetAllOverridesTo(enabled); 
    }

    public bool IsCRTEffectsEnabled()
    {
        return crtEffectsEnabled;
    }

    // MODE VISEUR
    public void SetAimMode(bool enabled)
    {
        aimReticuleMode = enabled;
    }
    public bool GetAimMode()
    {
        return aimReticuleMode;
    }


    //public void SetAdvancedShootingControls(bool enabled)
    //{
    //    if (enabled)
    //    {
    //        _aimingReticle.SetActive(true);
    //        _aimingReticle.transform.SetParent(null, false);
    //        _advancedShootingControls = true;
    //    }
    //    else
    //    {
    //        _aimingReticle.transform.SetParent(gameObject.transform);
    //        _aimingReticle.SetActive(false);
    //        _advancedShootingControls = false;
    //    }
    //}

}
