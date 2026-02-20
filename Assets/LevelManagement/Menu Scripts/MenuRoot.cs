using UnityEngine;

public class MenuRoot : MonoBehaviour
{
    private static MenuRoot instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // delete duplication
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

    }
}