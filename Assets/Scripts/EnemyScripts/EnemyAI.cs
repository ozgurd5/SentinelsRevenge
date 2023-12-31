using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Assign - Values")]
    [SerializeField] private float walkPointRange = 10f;
    [SerializeField] private float sightRange = 20f;
    public float attackRange = 7f;
    public float verticalAttackRange = 2f;
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;

    [Header("No Touch - Info")]
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool isWalkPointSet;
    [SerializeField] private bool isPlayerInSightRange;
    public bool isPlayerInAttackRange;
    public bool didEncounterPlayer;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private EnemyManager em;
    private EnemyCombatManager ecm;

    private int groundLayer = 1 << 6;
    private int playerLayer = 1 << 7;

    //TODO: AAAAA
    [Header("AAA")] public bool isTier3;
    public bool canTier3LookAtPlayer = true;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        em = GetComponent<EnemyManager>();
        ecm = GetComponent<EnemyCombatManager>();
    }

    private void Update()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isPlayerInAttackRange = Physics.CheckBox(transform.position, new Vector3(attackRange, verticalAttackRange, attackRange),
            Quaternion.identity, playerLayer);

        if (!isTier3 && em.enemyState == EnemyManager.EnemyState.Attacking)
        {
            transform.LookAt(playerTransform, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        if (em.enemyState is EnemyManager.EnemyState.Dead or EnemyManager.EnemyState.GettingDamage or EnemyManager.EnemyState.Attacking) return;

        if (!isPlayerInSightRange && !isPlayerInAttackRange && !didEncounterPlayer) Patrol();
        else if (isPlayerInAttackRange && isPlayerInSightRange) Attack();
        else if ((isPlayerInSightRange && !isPlayerInAttackRange) || didEncounterPlayer) Chase();
    }

    private void Patrol()
    {
        em.enemyState = EnemyManager.EnemyState.Walking;

        if (!isWalkPointSet) SearchWalkPoint();
        else if (isWalkPointSet) navMeshAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) isWalkPointSet = false;

        navMeshAgent.speed = walkingSpeed;
    }

    public float num = 0.2f;

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.CheckSphere(walkPoint, num, groundLayer)) isWalkPointSet = true;
    }

    private void Chase()
    {
        em.enemyState = EnemyManager.EnemyState.Chasing;

        didEncounterPlayer = true;

        navMeshAgent.SetDestination(playerTransform.position);
        navMeshAgent.speed = runningSpeed;
    }

    private bool isRotating;
    public bool canTier3DynamicallyLookAtPlayer = true;
    private void Attack()
    {
        ecm.Attack();

        navMeshAgent.SetDestination(transform.position); //Sudden stop

        if (canTier3LookAtPlayer) //TODO: AAAAA
        {
            if (!canTier3DynamicallyLookAtPlayer && !isRotating && isTier3) Rotate();
            if (canTier3DynamicallyLookAtPlayer)
            {
                transform.LookAt(playerTransform, Vector3.up);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            }
        }
    }

    private async void Rotate()
    {
        canTier3DynamicallyLookAtPlayer = false;
        isRotating = true;
        transform.DOLookAt(playerTransform.position, 0.2f, AxisConstraint.None, Vector3.up);
        await UniTask.WaitForSeconds(0.2f);
        isRotating = false;
        canTier3DynamicallyLookAtPlayer = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //if (isTier3) Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRange, verticalAttackRange, attackRange) * 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(walkPoint, 2f);
    }
}
