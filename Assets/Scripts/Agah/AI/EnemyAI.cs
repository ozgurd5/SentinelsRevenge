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
        None,
        Patrol,
        Chase,
        Attack
    }
    EnemyState enemyState;

    [SerializeField] float attackRange, sightRange;

    [SerializeField] LayerMask WhatIsPlayer, WhatIsGround; // WhatIsLove;


    private void Awake()
    {
        if (player == null) Debug.LogError("Player Not Initiliazed in Inspector!");
        agent = GetComponent<NavMeshAgent>();
    }

    void MakeAIWork()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
