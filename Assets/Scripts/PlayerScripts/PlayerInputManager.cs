using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static float sensitivity;

    private RobotInputActions pia;

    [Header("Info - No Touch")]
    public Vector2 lookInput;
    public Vector2 moveInput;
    public bool isRunKey;
    public bool isRunKeyDown;
    public bool isRunKeyUp;
    public bool isJumpKeyDown;
    //public bool isDashKeyDown;
    public bool isAimKeyDown;
    public bool isAimKeyUp;
    public bool isAttackKeyDown;

    private void Awake()
    {
        pia = new RobotInputActions();
        pia.Default.Enable();

        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
    }

    private void Update()
    {
        lookInput = pia.Default.Look.ReadValue<Vector2>() * sensitivity;
        moveInput = pia.Default.Movement.ReadValue<Vector2>();

        isRunKey = pia.Default.Run.IsPressed();
        isRunKeyDown = pia.Default.Run.WasPressedThisFrame();
        isRunKeyUp = pia.Default.Run.WasReleasedThisFrame();

        isJumpKeyDown = pia.Default.Jump.WasPressedThisFrame();
        //isDashKeyDown = pia.Default.Dash.WasPressedThisFrame();

        isAimKeyDown = pia.Default.Aim.WasPressedThisFrame();
        isAimKeyUp = pia.Default.Aim.WasReleasedThisFrame();
        isAttackKeyDown = pia.Default.Attack.WasPressedThisFrame();
    }
}