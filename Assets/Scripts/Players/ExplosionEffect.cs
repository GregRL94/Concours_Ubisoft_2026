using Unity.Cinemachine;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    void Awake()
    {
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }
    }
}
