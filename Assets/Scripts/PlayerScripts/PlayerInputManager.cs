using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static float sensitivity;

    private RobotInputActions pia;

    [Header("Info - No Touch")]
    public Vector2 lookInput;
    public Vector2 moveInput;
    public bool isJumpKeyDown;
    public bool isRunKey;
    public bool isRunKeyDown;
    public bool isRunKeyUp;
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
        lookInput.y *= -1; //Fixes y axis inversion
        moveInput = pia.Default.Movement.ReadValue<Vector2>();

        isJumpKeyDown = pia.Default.Jump.WasPressedThisFrame();

        isRunKey = pia.Default.Run.IsPressed();
        isRunKeyDown = pia.Default.Run.WasPressedThisFrame();
        isRunKeyUp = pia.Default.Run.WasReleasedThisFrame();

        isAimKeyDown = pia.Default.Aim.WasPressedThisFrame();
        isAimKeyUp = pia.Default.Aim.WasReleasedThisFrame();
        isAttackKeyDown = pia.Default.Attack.WasPressedThisFrame();
    }
}