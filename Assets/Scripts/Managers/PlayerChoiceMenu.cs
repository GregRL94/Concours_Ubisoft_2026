using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChoiceMenu : Menu
{
    public static PlayerChoiceMenu Instance;

    public enum PlayerSlot
    {
        Left,     // Movement
        Center,   // None
        Right     // Shoot
    }

    [System.Serializable]
    private class PlayerUI
    {
        public RectTransform selector;
        public TextMeshProUGUI roleText;

        [Header("Confirm Fill")]
        public Image confirmFill;

        [Header("Positions")]
        public Transform left;
        public Transform center;
        public Transform right;
    }

    [Header("Players UI")]
    [SerializeField] private PlayerUI player1;
    [SerializeField] private PlayerUI player2;

    [Header("Role Colors")]
    [SerializeField] private Color movementColor = Color.yellow;
    [SerializeField] private Color shootColor = Color.green;
    [SerializeField] private Color noneColor = Color.white;

    [Header("Confirm Config")]
    [SerializeField] private float confirmTime = 1.2f;
    [SerializeField] private string sceneName = "Mission1";
    //[SerializeField] private float shakeIntensity = 5f;

    private PlayerSlot p1Slot = PlayerSlot.Center;
    private PlayerSlot p2Slot = PlayerSlot.Center;

    private float p1Fill;
    private float p2Fill;

    private bool p1Holding;
    private bool p2Holding;

    private bool p1Locked;
    private bool p2Locked;

    [SerializeField] private FadeTransition fadeTransition;

    // --------------------------------------------------
    // INIT
    // --------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        p1Slot = PlayerSlot.Center;
        p2Slot = PlayerSlot.Center;

        UpdateSelectors();
        RefreshUI();

        player1.confirmFill.fillAmount = 0;
        player2.confirmFill.fillAmount = 0;
    }

    private void Update()
    {
        HandleFill(ref p1Fill, p1Holding, ref p1Locked, player1);
        HandleFill(ref p2Fill, p2Holding, ref p2Locked, player2);

    }


    private void OnEnable()
    {
        PlayerInputListener.UINavigate += OnNavigate;
        PlayerInputListener.UISubmit += OnSubmit;
        PlayerInputListener.UISubmitReleased += StopHolding;

        // Check si on est dans MainMenu
        if (!MenuManager.Instance.IsMainMenuScene())
        {
            Time.timeScale = 0f;
        }
    }

    private void OnDisable()
    {
        PlayerInputListener.UINavigate -= OnNavigate;
        PlayerInputListener.UISubmit -= OnSubmit;
        PlayerInputListener.UISubmitReleased -= StopHolding;

        if (!MenuManager.Instance.IsMainMenuScene())
        {
            Time.timeScale = 1f;
        }

        // Reset Locks quand on quitte le menu
        ResetLocks();
    }

    // INPUT LISTENER
    private void OnNavigate(int playerId, Vector2 dir)
    {
        if (Mathf.Abs(dir.x) < 0.9f)
            return;

        if (playerId == 0 && !p1Locked && !p1Holding)
            MoveHorizontal(ref p1Slot, dir.x);

        if (playerId == 1 && !p2Locked && !p2Holding)
            MoveHorizontal(ref p2Slot, dir.x);

        UpdateSelectors();
        RefreshUI();
    }

    private void OnSubmit(int playerId)
    {
        if (playerId == 0)
        {
            if (p1Locked || p1Holding)
                return;

            if (p1Slot == PlayerSlot.Center)
            {
                //AudioManager.Instance.PlaySound("UI_Error");
                return;
            }

            p1Holding = true;
        }

        if (playerId == 1)
        {
            if (p2Locked || p2Holding)
                return;

            if (p2Slot == PlayerSlot.Center)
            {
                //AudioManager.Instance.PlaySound("UI_Error");
                return;
            }

            p2Holding = true;
        }
    }

    // SELECTION LOGICS
    public void StopHolding(int playerId)
    {
        if (playerId == 0)
            p1Holding = false;

        if (playerId == 1)
            p2Holding = false;
    }

    public void CancelSelection(int playerId)
    {
        if (playerId == 0 && p1Locked)
        {
            p1Locked = false;
            p1Fill = 0;
            p1Holding = false;
            player1.confirmFill.fillAmount = 0;

            //AudioManager.Instance.PlaySound("UI_Cancel");
        }

        if (playerId == 1 && p2Locked)
        {
            p2Locked = false;
            p2Fill = 0;
            p2Holding = false;
            player2.confirmFill.fillAmount = 0;

            //AudioManager.Instance.PlaySound("UI_Cancel");
        }
    }

    public bool HasLockedSelection()
    {
        return p1Locked || p2Locked;
    }

    // LOGIC ANIMATION 
    private void HandleFill(ref float fill, bool holding, ref bool locked, PlayerUI ui)
    {
        if (locked)
            return;

        if (holding)
        {
            // Applique la couleur selon le rôle choisi
            ui.confirmFill.color = GetColorFromSlot(
                ui == player1 ? p1Slot : p2Slot
            );

            fill += Time.unscaledDeltaTime / confirmTime;
            fill = Mathf.Clamp01(fill);

            ui.confirmFill.fillAmount = fill;

            if (fill >= 1f)
            {
                locked = true;
                ui.confirmFill.fillAmount = 1f;

                AudioManager.Instance.PlaySound("UI_Submit");

                ValidateMatch();
            }
        }
        else
        {
            if (fill > 0)
            {
                fill = 0;
                ui.confirmFill.fillAmount = 0;
            }
        }
    }

    // NAVIGATION
    private void MoveHorizontal(ref PlayerSlot slot, float x)
    {
        if (x > 0) slot++;
        else slot--;

        if (slot > PlayerSlot.Right)
            slot = PlayerSlot.Left;

        if (slot < PlayerSlot.Left)
            slot = PlayerSlot.Right;

        AudioManager.Instance.PlaySound("UI_move_to_new_option");
    }

    // VALIDATION
    private void ValidateMatch()
    {
        if (!p1Locked || !p2Locked)
            return;

        bool validRoles =
            p1Slot != PlayerSlot.Center &&
            p2Slot != PlayerSlot.Center &&
            GetRole(p1Slot) != GetRole(p2Slot);

        if (validRoles)
        {
            StartGame();
        }
        else
        {
            //AudioManager.Instance.PlaySound("UI_Error");

            ResetLocks();
        }
    }

    private void ResetLocks()
    {
        p1Locked = false;
        p2Locked = false;

        p1Holding = false;
        p2Holding = false;

        p1Fill = 0;
        p2Fill = 0;

        player1.confirmFill.fillAmount = 0;
        player2.confirmFill.fillAmount = 0;

        p1Slot = PlayerSlot.Center;
        p2Slot = PlayerSlot.Center;

        UpdateSelectors();
        RefreshUI();
    }

    // UI
    private void UpdateSelectors()
    {
        player1.selector.position = GetPosition(player1, p1Slot);
        player2.selector.position = GetPosition(player2, p2Slot);
    }

    private Vector3 GetPosition(PlayerUI ui, PlayerSlot slot)
    {
        return slot switch
        {
            PlayerSlot.Left => ui.left.position,
            PlayerSlot.Center => ui.center.position,
            PlayerSlot.Right => ui.right.position,
            _ => ui.center.position
        };
    }

    private void RefreshUI()
    {
        player1.roleText.text = $"P1 : {GetRole(p1Slot)}";
        player2.roleText.text = $"P2 : {GetRole(p2Slot)}";

        player1.confirmFill.color = GetColorFromSlot(p1Slot);
        player2.confirmFill.color = GetColorFromSlot(p2Slot);
    }

    private string GetRole(PlayerSlot slot)
    {
        return slot switch
        {
            PlayerSlot.Left => "Movement",
            PlayerSlot.Right => "Shoot",
            _ => "None"
        };
    }

    // GAME START
    private void StartGame()
    {
        // Assigner les rôles
        PlayerRoleManager.Instance.AssignRoles(
            SlotToManagerRole(p1Slot),
            SlotToManagerRole(p2Slot)
        );

        // SI on est dans le Main Menu scene
        if (MenuManager.Instance.IsMainMenuScene())
        {
            TransitionManager.Instance.TransitionToScene(
                sceneName,
                fadeTransition,
                0.5f
            );
        }
        //else // pause menu
        //{
        //    // Refresh Input 
        //    PlayerInputInitializer initializer = FindObjectOfType<PlayerInputInitializer>();

        //    if (initializer != null)
        //    {
        //        initializer.ApplyRoles();
        //    }
        //    else
        //    {
        //        Debug.LogWarning("PlayerInputInitializer not found in scene!");
        //    }

        //    //Retour au pause menu
        //    MenuManager.Instance.CloseMenu();
        //    AudioManager.Instance.PlaySound("UI_Back");

        //}
    }
    private PlayerRole SlotToManagerRole(PlayerSlot slot)
    {
        return slot switch
        {
            PlayerSlot.Left => PlayerRole.Movement,
            PlayerSlot.Right => PlayerRole.Shoot,
            _ => PlayerRole.None
        };
    }
    private Color GetColorFromSlot(PlayerSlot slot)
    {
        return slot switch
        {
            PlayerSlot.Left => movementColor,
            PlayerSlot.Right => shootColor,
            _ => noneColor
        };
    }
}
