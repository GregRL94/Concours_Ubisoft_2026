using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FadeTransition gameOverTransitionPrefab;
    [SerializeField] private FadeTransition endGameTransitionPrefab;


    void Update()
    {
        // DEBUGS
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            TransitionManager.Instance.FadeInCurrentScene(gameOverTransitionPrefab, MenuManager.Instance.GetGameOverMenu(),0f);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TransitionManager.Instance.FadeInCurrentScene(endGameTransitionPrefab, MenuManager.Instance.GetEndMenu(),0f);
        }
    }

}