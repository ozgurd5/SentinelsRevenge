using UnityEngine;

public class PlayerCombatAudioManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private AudioSource aimSource;
    [SerializeField] private AudioSource meleeAttackSource;
    [SerializeField] private AudioSource rangedAttackSource;
    [SerializeField] private AudioSource getHitSource;
    [SerializeField] private AudioSource deathSource;

    public void ToggleAimSound(bool willPlay)
    {
        if (willPlay) aimSource.Play();
        else aimSource.Stop();
    }

    public void ToggleMeleeAttackSound(bool willPlay)
    {
        if (willPlay) meleeAttackSource.Play();
        else meleeAttackSource.Stop();
    }

    public void ToggleRangedAttackSound(bool willPlay)
    {
        if (willPlay) rangedAttackSource.Play();
        else rangedAttackSource.Stop();
    }

    public void ToggleGetHitSound(bool willPlay)
    {
        if (willPlay) getHitSource.Play();
        else getHitSource.Stop();
    }

    public void ToggleDeathSound(bool willPlay)
    {
        if (willPlay) deathSource.Play();
        else deathSource.Stop();
    }
}
