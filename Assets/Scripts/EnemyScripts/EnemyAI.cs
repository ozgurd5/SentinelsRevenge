using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Assign - Values")]
    [SerializeField] private float walkPointRange = 10f;
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;

    [Header("No Touch - Info")]
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool isWalkPointSet;
    [SerializeField] private bool isPlayerInSightRange;
    public bool isPlayerInAttackRange;
    [SerializeField] private bool didEncounterPlayer;
    [SerializeField] private Collider[] overlapSphereColliders;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private EnemyManager em;
    private EnemyCombatManager ecm;

    //private int groundLayer = 1 << 6;
    private int playerLayer = 1 << 7;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        em = GetComponent<EnemyManager>();
        ecm = GetComponent<EnemyCombatManager>();
    }

    private void Update()
    {
        if (em.enemyState is EnemyManager.EnemyState.Dead or EnemyManager.EnemyState.Waiting) return;

        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange && !didEncounterPlayer) Patrol();
        else if (isPlayerInAttackRange && isPlayerInSightRange) Attack();
        else if ((isPlayerInSightRange && !isPlayerInAttackRange) || didEncounterPlayer) Chase();
    }

    private void Patrol()
    {
        if (!isWalkPointSet) SearchWalkPoint();
        else if (isWalkPointSet) navMeshAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) isWalkPointSet = false;

        navMeshAgent.speed = walkingSpeed;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Walk point must must be in open space, not inside of an object. //Also checks (accidentally) if walkPoint in the air or not
        overlapSphereColliders = Physics.OverlapSphere(walkPoint, 2f); //Don't use non-alloc. Check the note in bellow
        if (overlapSphereColliders.Length > 1) isWalkPointSet = true;

        //The note in bellow: We must not use OverlapSphereNonAlloc because it doesn't count initial colliders. These are the ones I want to count..
        //..but it doesn't. I think it works like sphereCast. Unit docs note about sphereCast:
        //SphereCast will not detect colliders for which the sphere overlaps the collider.
    }

    private void Chase()
    {
        em.enemyState = EnemyManager.EnemyState.Chasing;

        didEncounterPlayer = true;

        navMeshAgent.SetDestination(playerTransform.position);
        navMeshAgent.speed = runningSpeed;
    }

    private void Attack()
    {
        ecm.Attack();

        navMeshAgent.SetDestination(transform.position); //Better than navMeshAgent.isStopped = true;

        transform.LookAt(playerTransform, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    //TODO: IMPLEMENT
    private void ResetAfterPlayerDeath()
    {
        didEncounterPlayer = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(walkPoint, 2f);
    }
}
