using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float meleeAttackAnimationTime = 0.8f;
    [SerializeField] private float rangedAttackAnimationTime;
    [SerializeField] private float rangedAttackCooldownTime;
    [SerializeField] private float aimModeSensitivityModifier = 0.5f;

    private PlayerExtensionData ped;
    private PlayerStateData psd;
    private PlayerInputManager pim;
    private CameraController cameraController;

    private bool isRangedAttackCooldownOver = true;

    private void Awake()
    {
        ped = GetComponent<PlayerExtensionData>();
        psd = GetComponent<PlayerStateData>();
        pim = GetComponent<PlayerInputManager>();
        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();
    }

    private void Update()
    {
        //TODO: MORE UNDERSTANDABLE IF STATEMENTS

        if (ped.hasGun && (pim.isAimKeyDown || pim.isAimKeyUp)) ToggleAim();

        if (psd.isAiming && pim.isAttackKeyDown && !psd.isRangedAttacking && isRangedAttackCooldownOver) RangedAttack();
        else if (ped.hasArms && !psd.isAiming && pim.isAttackKeyDown && !psd.isMeleeAttacking) MeleeAttack();
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
