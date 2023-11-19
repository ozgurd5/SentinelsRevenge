using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float punchAttackAnimationTime = 0.8f;
    [SerializeField] private float rangedAttackCooldownTime = 1f;
    [SerializeField] private float aimModeSensitivityModifier = 0.5f;

    private PlayerExtensionData ped;
    private PlayerStateData psd;
    private PlayerInputManager pim;
    private PlayerAnimationManager pam;
    private CrosshairManager cm;
    private CameraController cameraController;

    private bool isRangedAttackCooldownOver = true;
    private float rangedAttackAnimationTime;
    private float meleeAttackAnimationTime;

    private void Awake()
    {
        ped = GetComponent<PlayerExtensionData>();
        psd = GetComponent<PlayerStateData>();
        pim = GetComponent<PlayerInputManager>();
        pam = GetComponent<PlayerAnimationManager>();
        cm = GetComponent<CrosshairManager>();
        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();

        //Default value
        rangedAttackAnimationTime = pam.rangedAttackAnimationHalfDuration * 2;
        meleeAttackAnimationTime = pam.headbuttAttackAnimationHalfDuration * 2;
    }

    private void Update()
    {
        //TODO: MORE UNDERSTANDABLE IF STATEMENTS

        //TODO: EVENT
        if (ped.hasArms) meleeAttackAnimationTime = punchAttackAnimationTime;

        if (ped.hasGun && (pim.isAimKeyDown || pim.isAimKeyUp)) ToggleAim();

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
        }
    }

    private async void MeleeAttack()
    {
        psd.isMeleeAttacking = true;

        if (cm.canMeleeAttack) cm.damageable?.GetDamage(5, transform.forward);
        await UniTask.WaitForSeconds(meleeAttackAnimationTime);

        psd.isMeleeAttacking = false;
    }

    private async void RangedAttack()
    {
        psd.isRangedAttacking = true;
        StartRangedAttackCooldown();

        //if (cm.canRangedAttack) no need for that
        cm.damageable?.GetDamage(5, transform.forward);
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
