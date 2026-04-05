using System;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] private float _offsetAngleDeg = -90f;
    private GameObject _target;
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

    // Update is called once per frame
    void LateUpdate()
    {
        try
        {
            Vector2 targetDir = Target.transform.position - transform.position;
            transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(targetDir.x, targetDir.y, _offsetAngleDeg));
        }
        catch (Exception)
        {
            this.enabled = false;
            gameObject.SetActive(false);
        }        
    }
}
