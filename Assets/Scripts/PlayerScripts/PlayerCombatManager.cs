using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [Header("Assign - Durations")]
    [SerializeField] private float meleeAttackAnimationTime = 0.8f;
    [SerializeField] private float rangedAttackAnimationTime;
    [SerializeField] private float rangedAttackCooldownTime;
    [SerializeField] private float aimModeSensitivityModifier = 0.5f;

    private PlayerInputManager pim;
    private PlayerStateData psd;
    private CameraController cameraController;

    private bool isRangedAttackCooldownOver = true;

    private void Awake()
    {
        pim = GetComponent<PlayerInputManager>();
        psd = GetComponent<PlayerStateData>();
        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();
    }

    private void Update()
    {
        if (pim.isAimKeyDown || pim.isAimKeyUp) ToggleAim();

        if (psd.isAiming && pim.isAttackKeyDown && !psd.isRangedAttacking && isRangedAttackCooldownOver) RangedAttack();
        else if (!psd.isAiming && pim.isAttackKeyDown && !psd.isMeleeAttacking) MeleeAttack();
    }

    private void ToggleAim()
    {
        if (!psd.isAiming)
        {
            cameraController.ChangeCameraFov(CameraController.FovMode.AimFov);
            psd.isAiming = true;
            PlayerInputManager.sensitivity *= aimModeSensitivityModifier;
        }

        else
        {
            cameraController.ChangeCameraFov(CameraController.FovMode.DefaultFov);
            psd.isAiming = false;
            PlayerInputManager.sensitivity /= aimModeSensitivityModifier;

            //When the player is aiming, character looks at aiming position. When character is not moving it's looking the last looking position so..
            //..when the player stop aiming while looking up or down, character stays looking that direction if it's not moving. That's ugly and..
            //..must not happen. This line prevents it.
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    private async void MeleeAttack()
    {
        psd.isMeleeAttacking = true;

        await UniTask.WaitForSeconds(meleeAttackAnimationTime);
        psd.isMeleeAttacking = false;
    }

    private async void RangedAttack()
    {
        psd.isRangedAttacking = true;
        StartRangedAttackCooldown();

        await UniTask.WaitForSeconds(rangedAttackAnimationTime);
        psd.isRangedAttacking = false;
    }

    private async void StartRangedAttackCooldown()
    {
        isRangedAttackCooldownOver = false;
        await UniTask.WaitForSeconds(rangedAttackCooldownTime);
        isRangedAttackCooldownOver = true;
    }
}
