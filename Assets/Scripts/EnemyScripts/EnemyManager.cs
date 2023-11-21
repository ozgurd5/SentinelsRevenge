using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyState enemyState;
    public enum EnemyState
    {
        Walking,
        Chasing,
        Attacking,
        Waiting,
        GettingDamage,
        Dead
    }

    private Animator an;

    private void Awake()
    {
        an = GetComponent<Animator>();
    }

    private void Update()
    {
        if (enemyState == EnemyState.Walking) an.Play("Walk");
        else if (enemyState == EnemyState.Chasing) an.Play("Chase");
        else if (enemyState == EnemyState.Attacking) an.Play("Attack");
        else if (enemyState == EnemyState.Waiting) an.Play("Wait");
        else if (enemyState == EnemyState.Dead) an.Play("Death");
    }
}
