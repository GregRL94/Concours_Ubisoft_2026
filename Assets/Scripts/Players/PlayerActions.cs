using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [field: SerializeField] public PlayerType PlayerOfType { get; set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }
    [SerializeField] private float _offsetDeg;

    private UniversalPlayerControls _universalPlayerControls;
    private Vector2 _prevNonZeroInputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent<UniversalPlayerControls>(out _universalPlayerControls))
        {
            Debug.LogError("UniversalPlayerControls component not found on the GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        PerformDPadAction(_universalPlayerControls.DpadInputs);
    }

    public void PerformDPadAction(Vector2 dpadInput)
    {
        if (dpadInput != Vector2.zero)
        {
            _prevNonZeroInputs = dpadInput;
        }

        switch (PlayerOfType)
        {
            case PlayerType.Movement:
                transform.Translate(MovementSpeed * Time.deltaTime * new Vector2(dpadInput.x, dpadInput.y));
                break;

            case PlayerType.Artillery:
                transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngle(_prevNonZeroInputs.x, _prevNonZeroInputs.y, _offsetDeg));
                break;
        }
    }

    public void PerformMainAbility()
    {
        switch (PlayerOfType)
        {
            case PlayerType.Movement:
                Debug.Log("Performing main ability for Movement player");
                break;

            case PlayerType.Artillery:
                Debug.Log("Performing main ability for Artillery player");
                break;
        }
    }

    public void PerformSecondaryAbility()
    {
        switch (PlayerOfType)
        {
            case PlayerType.Movement:
                Debug.Log("Performing secondary ability for Movement player");
                break;

            case PlayerType.Artillery:
                Debug.Log("Performing secondary ability for Artillery player");
                break;
        }
    }
}
