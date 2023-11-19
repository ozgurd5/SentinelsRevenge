using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    [Header("Assign")]
    [SerializeField] private int health;
    [SerializeField] private int damage;

    [Header("Assign - Attack Animation")]
    [SerializeField] private float attackAnimationPrePrepareTime = 0.3f;
    [SerializeField] private float attackAnimationPrepareTime = 0.3f;
    [SerializeField] private float attackAnimationTime = 1f;
    [SerializeField] private float attackCooldownTime = 2f;
    [SerializeField] private float knockBackAmount = 2f;
    [SerializeField] private float chasingKnockBackModifier = 200f;
    [SerializeField] private float knockBackDuration = 0.2f;

    private EnemyManager em;
    private NavMeshAgent nma;
    private IDamageable playerDamageable;

    private bool isAttackCooldownOver = true;

    public event Action<int> OnDamageTaken;

    private void Awake()
    {
        em = GetComponent<EnemyManager>();
        nma = GetComponent<NavMeshAgent>();
        playerDamageable = GameObject.Find("Player").GetComponent<IDamageable>();
    }

    public async void Attack()
    {
        if (!isAttackCooldownOver) return;

        StartAttackCooldown();

        em.enemyState = EnemyManager.EnemyState.Waiting;
        await UniTask.WaitForSeconds(attackAnimationPrePrepareTime);

        em.enemyState = EnemyManager.EnemyState.Attacking;

        await UniTask.WaitForSeconds(attackAnimationPrepareTime);
        if (em.enemyState == EnemyManager.EnemyState.Dead) return; //Explanation is in down
        playerDamageable.GetDamage(damage, transform.forward);

        await UniTask.WaitForSeconds(attackAnimationTime - attackAnimationPrepareTime);
        if (em.enemyState == EnemyManager.EnemyState.Dead) return; //Explanation is in down
        em.enemyState = EnemyManager.EnemyState.Walking;

        //Explanation: When enemy decided to attack the player, it may die while attacking async operation is in process. That results in resurrection and..
        //..must not be happen.
    }

    private async void StartAttackCooldown()
    {
        isAttackCooldownOver = false;
        await UniTask.WaitForSeconds(attackCooldownTime);
        isAttackCooldownOver = true;
    }

    public async void GetDamage(int damageTakenAmount, Vector3 attackerTransformForward)
    {
        em.enemyState = EnemyManager.EnemyState.Waiting;

        nma.SetDestination(transform.position); //Sudden stop
        nma.isStopped = true;

        health -= damageTakenAmount;
        OnDamageTaken?.Invoke(health);

        if (CheckForDeath()) return;

        PlayKnockBackAnimation(attackerTransformForward);
        await UniTask.WaitForSeconds(knockBackDuration);

        nma.isStopped = false;
        em.enemyState = EnemyManager.EnemyState.Walking;
    }

    private async void PlayKnockBackAnimation(Vector3 attackerTransformForward)
    {
        //When enemy is chasing the player, its moving fast so must knocked back more, but when walking it must knocked back less.
        float localKnockBackAmount;
        if (em.enemyState == EnemyManager.EnemyState.Chasing) localKnockBackAmount = knockBackAmount * chasingKnockBackModifier;
        else localKnockBackAmount = knockBackAmount;

        float animationSpeed = localKnockBackAmount / knockBackDuration;
        float movingDistance = 0f;

        while (movingDistance < localKnockBackAmount)
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
            em.enemyState = EnemyManager.EnemyState.Dead;
            return true;
        }
        return false;
    }
}