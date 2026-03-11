using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private Selectable firstSelected;
    private Selectable[] selectables;

    private void Awake()
    {
        selectables = GetComponentsInChildren<Selectable>();
    }

    private void Start()
    {
        for (int i = 0; i < selectables.Length; i++)
        {
            Navigation nav = selectables[i].navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnUp = selectables[(i - 1 + selectables.Length) % selectables.Length];
            nav.selectOnDown = selectables[(i + 1) % selectables.Length];

            selectables[i].navigation = nav;
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }

}
