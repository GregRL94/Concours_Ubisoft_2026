using UnityEngine;

public interface IHit
{
    /// <summary>
    /// Simple hit with damage
    /// </summary>
    /// <param name="damage">The amount of damage to apply to the target. Must be non-negative.</param>
    public void OnHit(float damage); // Simple hit with damage

    /// <summary>
    /// Applies a repelling force to the target in a specified direction.
    /// </summary>
    /// <param name="repelForce">The magnitude of the force used to repel the target. Must be non-negative.</param>
    /// <param name="repelDirection">The direction in which the repelling force is applied, represented as a normalized vector.</param>
    public void OnHitRepel(float repelForce, Vector2 repelDirection); // Hit that also applies a repel force in a specific direction

    /// <summary>
    /// Applies a hit that causes a stun effect for a specified duration.
    /// </summary>
    /// <param name="stunDuration">The duration, in seconds, for which the stun effect is applied. Must be non-negative.</param>
    public void OnHitStun(float stunDuration); // Hit that applies a stun effect for a certain duration
}
