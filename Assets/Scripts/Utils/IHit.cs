using UnityEngine;

public interface IHit
{
    public void OnHit(float damage);
    public void OnHit(float damage, float repelForce, Vector2 repelDirection);
}
