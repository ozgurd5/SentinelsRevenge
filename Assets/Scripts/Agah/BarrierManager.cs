using System;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] GameObject barrier;
    [SerializeField] ParticleSystem barrierPopFX;
    [SerializeField] Vector3 boxBounds;

    [Header("Info - No Touch")]
    [SerializeField] private Collider[] enemies;
    [SerializeField] private int aliveEnemies;

    private int layerMask = 1 << 8;

    private void Start()
    {
        enemies = Physics.OverlapBox(transform.position, boxBounds, Quaternion.identity, layerMask);
    }

    private void Update()
    {
        if (!barrier.activeSelf) return;

        aliveEnemies = enemies.Length;
        foreach (Collider enemy in enemies)
        {
            if (enemy.GetComponent<EnemyManager>().enemyState == EnemyManager.EnemyState.Dead) aliveEnemies--;
        }

        if (aliveEnemies == 0)
        {
            barrier.SetActive(false);
            barrierPopFX.Play();
        }
    }
}
