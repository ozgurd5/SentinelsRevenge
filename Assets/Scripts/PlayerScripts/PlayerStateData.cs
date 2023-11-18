using UnityEngine;

public class PlayerStateData : MonoBehaviour
{
    [Header("Info - No Touch")]
    public PlayerMainState playerMainState = PlayerMainState.Normal;
    public bool isIdle;
    public bool isMoving;
    public bool isWalking;
    public bool isRunning;
    public bool isGrounded;
    public bool isJumping;
    public bool isDashing;

    public enum PlayerMainState
    {
        Normal,
        Paused
    }
}
