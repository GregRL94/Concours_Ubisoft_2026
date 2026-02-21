using UnityEngine;
using UnityEngine.InputSystem;


public class CreditsMenu : Menu
{
    public override void OnBack()
    {
        AudioManager.Instance.PlaySound("UI_Back");
        base.OnBack();
    }

}
