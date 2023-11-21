using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    [Header("Assign - Tier3 Only")]
    [SerializeField] private bool isTier3;
    [SerializeField] private Transform gunLineOutTransform;

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
    private EnemyAI eai;
    private EnemySoundManager esm;
    private ParticleEffectOnDeath peod;

    private NavMeshAgent nma;
    private LineRenderer gunLineRenderer;
    private IDamageable playerDamageable;

    private bool isAttackCooldownOver = true;

    //TODO: clean
    private int originalHealth;

    public event Action<int> OnDamageTaken;

    private void Awake()
    {
        em = GetComponent<EnemyManager>();
        eai = GetComponent<EnemyAI>();
        nma = GetComponent<NavMeshAgent>();
        esm = GetComponent<EnemySoundManager>();
        if (isTier3) gunLineRenderer = gunLineOutTransform.GetComponent<LineRenderer>();
        playerDamageable = GameObject.Find("Player").GetComponent<IDamageable>();
        peod = GetComponent<ParticleEffectOnDeath>();

        PlayerCombatManager.OnPlayerDeath += OnPlayerDeath;

        //TODO: clean
        originalHealth = health;
    }

    public async void Attack()
    {
        em.enemyState = EnemyManager.EnemyState.Waiting;

        if (!isAttackCooldownOver) return;
        StartAttackCooldown();

        if (isTier3)
        {
            eai.canTier3LookAtPlayer = false;
            eai.canTier3DynamicallyLookAtPlayer = false;
        }

        await UniTask.WaitForSeconds(attackAnimationPrePrepareTime);
        if (em.enemyState == EnemyManager.EnemyState.Dead) return; //Explanation is in down

        if (!eai.isPlayerInAttackRange) return; //TODO: AAAA

        em.enemyState = EnemyManager.EnemyState.Attacking;

        await UniTask.WaitForSeconds(attackAnimationPrepareTime);
        if (em.enemyState == EnemyManager.EnemyState.Dead) return; //Explanation is in down

        if (!IsPlayerEscaped()) playerDamageable.GetDamage(damage, transform.forward);

        if (isTier3) ShootLaser();

        await UniTask.WaitForSeconds(attackAnimationTime - attackAnimationPrepareTime);
        if (em.enemyState == EnemyManager.EnemyState.Dead) return; //Explanation is in down

        em.enemyState = EnemyManager.EnemyState.Waiting;

        //TODO: AAA
        eai.canTier3LookAtPlayer = true;

        //Explanation: When enemy decided to attack the player, it may die while attacking async operation is in process. That results in resurrection and..
        //..must not be happen.
    }

    private async void StartAttackCooldown()
    {
        isAttackCooldownOver = false;
        await UniTask.WaitForSeconds(attackCooldownTime);
        isAttackCooldownOver = true;
    }

    //TODO: clean
    private bool isHitTarget;
    private Transform hitTargetTransform;

    private bool IsPlayerEscaped()
    {
        if (!eai.isPlayerInAttackRange) return true;
        if (!isTier3) return !eai.isPlayerInAttackRange;

        isHitTarget = false;
        RaycastHit hit;
        if (!Physics.Raycast(gunLineOutTransform.position, transform.forward, out hit, eai.attackRange * math.sqrt(2))) return true;

        if (hit.collider.CompareTag("Player"))
        {
            hitTargetTransform = playerDamageable.GetTransform();
            isHitTarget = true;
            return false;
        }

        if (hit.collider.CompareTag("Enemy"))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            damageable.GetDamage(damage, transform.forward);
            hitTargetTransform = damageable.GetTransform();
            isHitTarget = true;
            return true;
        }

        else return true;
    }

    private async void ShootLaser()
    {
        gunLineRenderer.enabled = true;
        esm.ToggleRangedAttackSound(true);
        await UniTask.WaitForSeconds(attackAnimationTime - attackAnimationPrepareTime);
        gunLineRenderer.enabled = false;
    }

    private Vector3 positionOffset = new Vector3(0f, 1f, 0f);
    private void Update()
    {
        if (isTier3 && gunLineRenderer.enabled)
        {
            Vector3 endPosition;
            if (isHitTarget) endPosition = hitTargetTransform.position + positionOffset;
            else endPosition = gunLineOutTransform.position + transform.forward * eai.attackRange;

            gunLineRenderer.SetPosition(0, gunLineOutTransform.position);
            gunLineRenderer.SetPosition(1, endPosition);
        }
    }

    public async void GetDamage(int damageTakenAmount, Vector3 attackerTransformForward)
    {
        em.enemyState = EnemyManager.EnemyState.GettingDamage;

        nma.SetDestination(transform.position); //Sudden stop
        nma.isStopped = true;

        health -= damageTakenAmount;
        OnDamageTaken?.Invoke(health);

        if (CheckForDeath()) return;

        PlayKnockBackAnimation(attackerTransformForward);
        await UniTask.WaitForSeconds(knockBackDuration);

        nma.isStopped = false;
        em.enemyState = EnemyManager.EnemyState.Waiting;
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
            esm.ToggleDeathSound(true);

            Collider collider = GetComponent<Collider>();
            collider.enabled = false;

            peod.PlayDeathParticle();

            return true;
        }

        return false;
    }

    private void OnPlayerDeath()
    {
        eai.didEncounterPlayer = false;
        health = originalHealth;
        OnDamageTaken?.Invoke(health);
    }

    private void OnDestroy()
    {
        PlayerCombatManager.OnPlayerDeath -= OnPlayerDeath;
    }

    //This is stupid v2 3/3
    public Transform GetTransform()
    {
        return transform;
    }
}