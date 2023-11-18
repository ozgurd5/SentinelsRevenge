using Cysharp.Threading.Tasks;
using UnityEngine;

//TODO: BLEND TREE AND EVENT DRIVEN STATES
public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float rangedAttackAnimationMovingAmount = 20f;
    public float rangedAttackAnimationHalfDuration = 0.2f;

    private PlayerExtensionData ped;
    private PlayerStateData psd;

    private Animator headAnimator;
    private Animator armsAnimator;
    private Animator gunAnimator;

    private bool isRangedAttackAnimationPlaying;

    private void Awake()
    {
        ped = GetComponent<PlayerExtensionData>();
        psd = GetComponent<PlayerStateData>();

        headAnimator = transform.GetChild(0).GetComponent<Animator>();
        armsAnimator = transform.GetChild(1).GetComponent<Animator>();
        gunAnimator = transform.GetChild(2).GetComponent<Animator>();
    }

    private void Update()
    {
        if (psd.isMeleeAttacking) PlayAnimation("MeleeAttack");
        else if (psd.isRangedAttacking && !isRangedAttackAnimationPlaying) PlayRangedAttackAnimation();
        else if (psd.isIdle) PlayAnimation("Idle");
        else if (psd.isWalking) PlayAnimation("Walk");
        else if (psd.isRunning) PlayAnimation("Run");

        if (Input.GetKeyDown(KeyCode.E)) Debug.Log(transform.eulerAngles.x);
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 temp = transform.eulerAngles;
            temp.x = -20;
            transform.eulerAngles = temp;
        }
    }

    private void PlayAnimation(string animationName)
    {
        headAnimator.Play(animationName);
        if (ped.hasArms) armsAnimator.Play(animationName);
        if (ped.hasGun) gunAnimator.Play(animationName);
    }

    private void ToggleAnimators()
    {
        headAnimator.enabled = !headAnimator.enabled;
        armsAnimator.enabled = !armsAnimator.enabled;
        gunAnimator.enabled = !gunAnimator.enabled;
    }

    private async void PlayRangedAttackAnimation()
    {
        isRangedAttackAnimationPlaying = true;
        ToggleAnimators();

        float animationSpeed = rangedAttackAnimationMovingAmount / rangedAttackAnimationHalfDuration;
        float movingAmount = 0f;
        Vector3 tempRotation;

        while (movingAmount < rangedAttackAnimationMovingAmount)
        {
            movingAmount += Time.deltaTime * animationSpeed;

            tempRotation = transform.eulerAngles;
            tempRotation.x -= Time.deltaTime * animationSpeed;
            transform.eulerAngles = tempRotation;

            await UniTask.NextFrame();
        }

        movingAmount = 0f;

        Vector3 clampedRotation = transform.eulerAngles;
        clampedRotation.x = -rangedAttackAnimationMovingAmount;
        transform.eulerAngles = clampedRotation;

        while (movingAmount < rangedAttackAnimationMovingAmount)
        {
            movingAmount += Time.deltaTime * animationSpeed;

            tempRotation = transform.eulerAngles;
            tempRotation.x += Time.deltaTime * animationSpeed;
            transform.eulerAngles = tempRotation;

            await UniTask.NextFrame();
        }

        clampedRotation = transform.eulerAngles;
        clampedRotation.x = 0f;
        transform.eulerAngles = clampedRotation;

        ToggleAnimators();
        isRangedAttackAnimationPlaying = false;
    }
}
