using System;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] private float _offsetAngleDeg = -90f;
    private GameObject _target;
    private SpriteRenderer _arrowSpriteRender;
    private float _arrowDist;
    public GameObject Target
    {
        get => _target;
        set
        {
            _target = value;
            if (_target != null)
            {
                this.enabled = true;
                gameObject.SetActive(true);
            }
            else
            {
                this.enabled = false;
                gameObject.SetActive(false);
            }                
        }
    }

    private void Start()
    {
        // Verify if the arrow isn't too close from the target. If it is too close, the arrow disapear.
        // 1.1f is a small offset
        _arrowDist = gameObject.transform.GetChild(0).transform.localPosition.y * 1.1f;
        _arrowSpriteRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        try
        {
	        if (Vector2.Distance(Target.transform.position, transform.position) > _arrowDist)
	        {
            	    Vector2 targetDir = Target.transform.position - transform.position;
            	    transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(targetDir.x, targetDir.y, _offsetAngleDeg));
                    _arrowSpriteRender.enabled = true;
	        }
	        else 
	        {
                    _arrowSpriteRender.enabled = false;
            }
        }
        catch (Exception)
        {
            this.enabled = false;
            gameObject.SetActive(false);
        }        
    }
}
