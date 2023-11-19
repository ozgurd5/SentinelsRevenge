using System;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource runSource;
    [SerializeField] private AudioSource rangedAttackSource;
    [SerializeField] private AudioSource deathSource;

    private EnemyManager em;

    private bool isWalkSourcePlaying;
    private bool isRunSourcePlaying;

    private void Awake()
    {
        em = GetComponent<EnemyManager>();
    }

    private void Update()
    {
        //TODO: FIX THIS MONSTROSITY

        //Walk - Run Group
        if (em.enemyState is EnemyManager.EnemyState.Dead or EnemyManager.EnemyState.Waiting)
        {
            isWalkSourcePlaying = false;
            walkSource.Stop();

            isRunSourcePlaying = false;
            runSource.Stop();
        }
        else if (em.enemyState == EnemyManager.EnemyState.Walking && !isWalkSourcePlaying)
        {
            isWalkSourcePlaying = true;
            walkSource.Play();

            isRunSourcePlaying = false;
            runSource.Stop();
        }
        else if (em.enemyState == EnemyManager.EnemyState.Chasing && !isRunSourcePlaying)
        {
            isWalkSourcePlaying = false;
            walkSource.Stop();

            isRunSourcePlaying = true;
            runSource.Play();
        }
    }

    public void ToggleRangedAttackSound(bool willPlay)
    {
        if (willPlay) rangedAttackSource.Play();
        else rangedAttackSource.Stop();
    }

    public void ToggleDeathSound(bool willPlay)
    {
        if (willPlay) deathSource.Play();
        else deathSource.Stop();
    }
}
