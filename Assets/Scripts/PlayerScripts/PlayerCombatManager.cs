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

    [Header("Assign")] [SerializeField] private Transform gunLineOutTransform;

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

    private Camera mainCamera;
    private CameraController cameraController;
    private LineRenderer gunLineRenderer;

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

        mainCamera = Camera.main;
        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();
        gunLineRenderer = gunLineOutTransform.GetComponent<LineRenderer>();

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

        gunLineRenderer.SetPosition(0, gunLineOutTransform.position);
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

        gunLineRenderer.enabled = true;
        gunLineRenderer.SetPosition(1, GetMiddleOfTheScreen(40f)); //TODO: 40f IS RANGEDATTACK RANGE, MAKE IT VARIABLE

        cm.damageable?.GetDamage(gunDamage, transform.forward);
        await UniTask.WaitForSeconds(rangedAttackAnimationTime);

        gunLineRenderer.enabled = false;
        psd.isRangedAttacking = false;
    }

    private Vector3 GetMiddleOfTheScreen(float zValue)
    {
        Vector3 viewportMiddle = new Vector3(0.5f, 0.5f, zValue);
        return mainCamera.ViewportToWorldPoint(viewportMiddle);
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

    //This is stupid v2 2/3
    public Transform GetTransform()
    {
        return transform;
    }
}