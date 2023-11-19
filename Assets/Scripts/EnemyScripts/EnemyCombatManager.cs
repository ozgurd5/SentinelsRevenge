using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    [Header("Assign")]
    [SerializeField] private float rangedAttackAnimationTime = 1f;
    [SerializeField] private float rangedAttackCooldownTime = 2f;

    private EnemyManager em;

    private bool isAttackCooldownOver = true;

    private void Awake()
    {
        em = GetComponent<EnemyManager>();
    }

    public async void Attack()
    {
        if (!isAttackCooldownOver) return;

        StartAttackCooldown();
        em.enemyState = EnemyManager.EnemyState.Attacking;
        await UniTask.WaitForSeconds(rangedAttackAnimationTime);

        em.enemyState = EnemyManager.EnemyState.Waiting;
    }

    private async void StartAttackCooldown()
    {
        isAttackCooldownOver = false;
        await UniTask.WaitForSeconds(rangedAttackCooldownTime);
        isAttackCooldownOver = true;
    }

    public void GetDamage()
    {

    }
}
