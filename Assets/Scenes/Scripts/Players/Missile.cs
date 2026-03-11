using UnityEngine;

public class Missile : MonoBehaviour
{
    private float _speed;
    private float _rotationSpeed = 50f;
    private float _lifetime;
    private float _damage;
    [SerializeField] private GameObject _target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Missile created with target: " + _target);
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsTarget();
    }

    public void SetupMissile(float t)
    {

    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    private void RotateTowardsTarget()
    {
        if (_target == null) { return; }
        Vector2 dir = _target.transform.position - transform.position;
        float targetAngle = MathUtils.DirToAngleRad(dir.x, dir.y, -90f);
        Vector3 targetRotation = new Vector3(0f, 0f, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), _rotationSpeed * Time.deltaTime);
        Debug.Log("Rotating towards target" + transform.rotation);
    }
}
