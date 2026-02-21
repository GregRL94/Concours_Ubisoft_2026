using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public PlayerType PlayerOfType { get; set; }
    private UniversalPlayerControls _universalPlayerControls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _universalPlayerControls = GetComponent<UniversalPlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        PerformDPadAction(_universalPlayerControls.DpadInputs);
    }

    public void SetPlayerType(PlayerType playerType)
    {
        PlayerOfType = playerType;
    }

    public void PerformDPadAction(Vector2 dpadInput)
    {
        // Implement logic to perform actions based on D-pad input and player type
    }

    public void PerformMainAbility()
    {
        // Implement logic to perform the main ability based on the player type
    }

    public void PerformSecondaryAbility()
    {
        // Implement logic to perform the secondary ability based on the player type
    }
}
