using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private int health = 10;
    [SerializeField] private int damage = 3;

    public enum EnemyState
    {
        Walking,
        Chasing,
        Attacking,
        GettingDamage,
        Dead
    }

    public EnemyState enemyState;
}
