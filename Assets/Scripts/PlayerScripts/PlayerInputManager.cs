using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private RobotInputActions pia;

    [Header("Info - No Touch")]
    public Vector2 lookInput;
    public Vector2 moveInput;
    public bool isRunKey;
    public bool isJumpKeyDown;
    //public bool isDashKeyDown;

    private void Awake()
    {
        pia = new RobotInputActions();
        pia.Default.Enable();
    }

    private void Update()
    {
        lookInput = pia.Default.Look.ReadValue<Vector2>(); //TODO: SENSITIVITY
        moveInput = pia.Default.Movement.ReadValue<Vector2>();

        isRunKey = pia.Default.Run.IsPressed();
        isJumpKeyDown = pia.Default.Jump.WasPressedThisFrame();
        //isDashKeyDown = pia.Default.Dash.WasPressedThisFrame();
    }
}