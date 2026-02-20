using UnityEngine;
using DG.Tweening;

public interface ITransition
{
    Tween PlayIn();
    Tween PlayOut();
}
