using UnityEngine;

//TODO: BLEND TREE AND EVENT DRIVEN STATES
public class PlayerAnimationManager : MonoBehaviour
{
    private PlayerExtensionData ped;
    private PlayerStateData psd;

    private Animator headAnimator;
    private Animator armsAnimator;
    private Animator gunAnimator;

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
        else if (psd.isRangedAttacking) PlayAnimation("RangedAttack");
        else if (psd.isIdle) PlayAnimation("Idle");
        else if (psd.isWalking) PlayAnimation("Walk");
        else if (psd.isRunning) PlayAnimation("Run");
    }

    private void PlayAnimation(string animationName)
    {
        headAnimator.Play(animationName);
        if (ped.hasArms) armsAnimator.Play(animationName);
        if (ped.hasGun) gunAnimator.Play(animationName);
    }
}
