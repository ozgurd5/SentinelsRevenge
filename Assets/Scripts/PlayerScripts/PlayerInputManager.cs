using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static float sensitivity;

    private RobotInputActions pia;

    [Header("Info - No Touch")]
    public Vector2 lookInput;
    public Vector2 moveInput;
    public bool isRunKeyDown;
    public bool isRunKey;
    public bool isRunKeyUp;
    public bool isJumpKeyDown;
    //public bool isDashKeyDown;

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

        isRunKeyDown = pia.Default.Run.WasPressedThisFrame();
        isRunKey = pia.Default.Run.IsPressed();
        isRunKeyUp = pia.Default.Run.WasReleasedThisFrame();

        isJumpKeyDown = pia.Default.Jump.WasPressedThisFrame();
        //isDashKeyDown = pia.Default.Dash.WasPressedThisFrame();
    }
}