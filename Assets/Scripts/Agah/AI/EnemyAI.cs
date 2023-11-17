using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject player;

    //patrol
    [SerializeField] float walkPointRange;
    bool walkPointSet;
    public Vector3 walkPoint;

    //attack
    [SerializeField] float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }
    EnemyState state;
    public bool playerInSightRange, playerInAttackRange;


    [SerializeField] float attackRange, sightRange;

    [SerializeField] LayerMask WhatIsPlayer, WhatIsGround; // WhatIsLove;


    private void Awake()
    {
        if (player == null) Debug.LogError("Player Not Initiliazed in Inspector!");
        agent = GetComponent<NavMeshAgent>();
        state = EnemyState.Patrol;  
        alreadyAttacked = false;
        walkPointSet = false;
    }

    void StateMachine()
    {
        switch (state)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
            default: state = EnemyState.Patrol; break;

        }
    }

    private void Attack()
    {
        Debug.Log("ATTACK");

        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Debug.Log("AttackPlayer");
            alreadyAttacked = true;
        }
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Chase()
    { 
        Debug.Log("CHASE"); 
        agent.SetDestination(player.transform.position);
    }

    void Patrol()
    { 
        Debug.Log("PATROL");
        if(!walkPointSet)
        {
            SetWalkPoint();
        }
        else agent.SetDestination(walkPoint);

        Vector3 dToWalkPoint = transform.position - walkPoint;
        Debug.Log("distance to walk point = " + dToWalkPoint);

        if (dToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        Debug.Log("WALKPOÝNT =" + walkPoint);
        if(Physics.Raycast(walkPoint, -transform.up, 3f, WhatIsGround))
        {
            Debug.Log("FÝZÝKS ÝSSUE");
            walkPointSet = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);
        //Debug.Log("player is in attack range? =>" + playerInAttackRange);
        if(playerInAttackRange)
        {
            state = EnemyState.Attack;
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        //Debug.Log("player is in sight range? =>" + playerInSightRange);
        if (playerInSightRange && !playerInAttackRange)
        {
            state = EnemyState.Chase;
        }
        StateMachine();
    }
}
