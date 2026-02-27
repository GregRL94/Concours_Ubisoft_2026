using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerRoleManager : MonoBehaviour
{
    public static PlayerRoleManager Instance;

    [field: SerializeField] public PlayerRole Player1Role { get; private set; } = PlayerRole.None;
    [field: SerializeField] public PlayerRole Player2Role { get; private set; } = PlayerRole.None;

    public Gamepad Player1Gamepad { get; private set; }
    public Gamepad Player2Gamepad { get; private set; }

    private List<Gamepad> connectedGamepads = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        RefreshGamepads();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    // DEVICE TRACKING 
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad gamepad)
            return;

        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                Debug.Log(" Gamepad Connected " + gamepad);
                RefreshGamepads();
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                Debug.Log(" Gamepad Disconnected " + gamepad);
                RefreshGamepads();
                break;
        }
    }

    private void RefreshGamepads()
    {
        connectedGamepads.Clear();

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.added)
                connectedGamepads.Add(gamepad);
        }

        AssignAvailableGamepads();
    }

    private void AssignAvailableGamepads()
    {
        Player1Gamepad = connectedGamepads.Count > 0 ? connectedGamepads[0] : null;
        Player2Gamepad = connectedGamepads.Count > 1 ? connectedGamepads[1] : null;

        Debug.Log($" Active Pads  P1: {Player1Gamepad} | P2: {Player2Gamepad}");
    }

    // ROLE LOGIC
    public void AssignRoles(PlayerRole p1Role, PlayerRole p2Role)
    {
        Player1Role = p1Role;
        Player2Role = p2Role;

        Debug.Log($" Roles  P1: {p1Role} | P2: {p2Role}");
    }

    public bool AreRolesValid()
    {
        if (Player1Role == PlayerRole.None || Player2Role == PlayerRole.None) 
            return false;

        if (Player1Role == Player2Role)
            return false;

        if (Player1Gamepad == null || Player2Gamepad == null)
            return false;

        return true;
    }
}