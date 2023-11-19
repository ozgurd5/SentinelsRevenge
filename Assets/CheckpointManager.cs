using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    [SerializeField] GameObject player;

    public Vector3 lastCheckpoint;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        PlayerCombatManager.OnPlayerDeath += PlayerCombatManager_OnPlayerDeath;
    }

    private void PlayerCombatManager_OnPlayerDeath()
    {
        player.transform.position = lastCheckpoint;
    }
}
