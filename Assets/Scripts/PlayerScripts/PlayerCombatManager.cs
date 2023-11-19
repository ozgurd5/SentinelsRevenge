using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour, IDamageable
{
    public static event Action OnPlayerDeath;

    [Header("Assign")]
    [SerializeField] private int health = 10;
    [SerializeField] private int headDamage = 1;
    [SerializeField] private int armsDamage = 3;
    [SerializeField] private int gunDamage = 5;

    [Header("Assign")]
    [SerializeField] private float punchAttackAnimationPrepareTime = 0.3f;
    [SerializeField] private float punchAttackAnimationTime = 0.8f;
    [SerializeField] private float rangedAttackCooldownTime = 1f;
    [SerializeField] private float aimModeSensitivityModifier = 0.5f;
    [SerializeField] private float knockBackAmount = 5f;
    [SerializeField] private float knockBackDuration = 0.2f;

    private PlayerExtensionData ped;
    private PlayerStateData psd;
    private PlayerInputManager pim;
    private PlayerAnimationManager pam;
    private CrosshairManager cm;
    private CameraController cameraController;

    private bool isRangedAttackCooldownOver = true;
    private float rangedAttackAnimationTime;
    private float meleeAttackAnimationTime;
    private float meleeAttackPrepareTime;
    private int meleeDamage;

    public event Action<int> OnHealthChanged;

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
        meleeAttackPrepareTime = pam.headbuttAttackAnimationHalfDuration;
        meleeDamage = headDamage;
    }

    private void Update()
    {
        //TODO: MORE UNDERSTANDABLE IF STATEMENTS

        //TODO: EVENT
        if (ped.hasArms)
        {
            meleeAttackAnimationTime = punchAttackAnimationTime;
            meleeAttackPrepareTime = punchAttackAnimationPrepareTime;
            meleeDamage = armsDamage;
        }

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

        await UniTask.WaitForSeconds(meleeAttackPrepareTime);
        if (cm.canMeleeAttack) cm.damageable?.GetDamage(meleeDamage, transform.forward);
        await UniTask.WaitForSeconds(meleeAttackAnimationTime - punchAttackAnimationPrepareTime);

        psd.isMeleeAttacking = false;
    }

    private async void RangedAttack()
    {
        psd.isRangedAttacking = true;
        StartRangedAttackCooldown();

        //if (cm.canRangedAttack) no need for that
        cm.damageable?.GetDamage(gunDamage, transform.forward);
        await UniTask.WaitForSeconds(rangedAttackAnimationTime);

        psd.isRangedAttacking = false;
    }

    private async void StartRangedAttackCooldown()
    {
        isRangedAttackCooldownOver = false;
        await UniTask.WaitForSeconds(rangedAttackCooldownTime);
        isRangedAttackCooldownOver = true;
    }

    public async void GetDamage(int damageTakenAmount, Vector3 attackerTransformForward)
    {
        health -= damageTakenAmount;
        OnHealthChanged?.Invoke(health);
        if (CheckForDeath()) return;

        PlayKnockBackAnimation(attackerTransformForward);
        await UniTask.WaitForSeconds(knockBackDuration);
    }

    //TODO: NO REPETITION
    private async void PlayKnockBackAnimation(Vector3 attackerTransformForward)
    {
        float animationSpeed = knockBackAmount / knockBackDuration;
        float movingDistance = 0f;

        while (movingDistance < knockBackAmount)
        {
            movingDistance += Time.deltaTime * animationSpeed;

            Vector3 tempPosition = transform.position + attackerTransformForward * (Time.deltaTime * animationSpeed);
            transform.position = tempPosition;

            await UniTask.NextFrame();
        }
    }

    private bool CheckForDeath()
    {
        if (health <= 0)
        {
            OnHealthChanged?.Invoke(10);
            OnPlayerDeath?.Invoke();
            return true;
        }
        return false;
    }
}