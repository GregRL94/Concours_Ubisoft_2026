using UnityEngine;

public class PressedThatTutoBtn : MonoBehaviour
{

	Transform BtnTop;
	Transform AbilityLogo;
	float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	BtnTop = this.gameObject.transform.GetChild(1);
	AbilityLogo = this.gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
	if (timer >= 0.5f) 
	{
		timer = 0.0f;
		if (BtnTop.localPosition.y <= 0.0f)
	       	{
			AbilityLogo.localPosition = new Vector3(0.0f,0.2f,0.0f);
			BtnTop.localPosition = new Vector3(0.0f,0.2f,0.0f);
		}
		else
		{
			AbilityLogo.localPosition = Vector3.zero;
			BtnTop.localPosition = Vector3.zero;
		}
	}
    }
}
