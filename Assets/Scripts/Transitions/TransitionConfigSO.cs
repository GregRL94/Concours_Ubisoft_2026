using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Transitions/Config")]
public class TransitionConfigSO : ScriptableObject
{
    public float duration = 0.5f;
    public Ease ease = Ease.InOutCubic;
}

