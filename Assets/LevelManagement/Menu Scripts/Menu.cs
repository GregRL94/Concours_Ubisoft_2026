using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public virtual void ShowDisplay()
    {
        if (this == null || gameObject == null)
            return;

        gameObject.SetActive(true);
    }

    public virtual void HideDisplay()
    {
        if (this == null || gameObject == null)
            return;

        gameObject.SetActive(false);
    }


    public virtual void OnBack()
    {
        MenuManager.Instance.CloseMenu();
    }
}


