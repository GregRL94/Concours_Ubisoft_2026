using UnityEngine;

public class PingPong : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public float duration = 1.0f;
    private float timer;

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            Destroy(gameObject);
            AudioManager.Instance.CrossFadeMusic("Music_Gameplay");
            return;
        }

        float t = Mathf.PingPong(Time.time / duration, 1.0f);
        float newScale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = Vector3.one * newScale;
    }

}

