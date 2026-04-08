using UnityEngine;
using UnityEngine.EventSystems;

public class UINavigationSoundSystem : MonoBehaviour
{
    private GameObject lastSelected;

    //private void Update()
    //{
    //    if (EventSystem.current == null)
    //        return;

    //    GameObject current = EventSystem.current.currentSelectedGameObject;

    //    if (current != lastSelected && current != null)
    //    {
    //        lastSelected = current;
    //        PlayNavigationSound();
    //    }
    //}

    public void PlayNavigationSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("UI_move_to_new_option");
    }
}