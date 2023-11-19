using UnityEngine;

public class AttackParticleEffect : MonoBehaviour
{
    [SerializeField]ParticleSystem headbuttParticle;
    [SerializeField] ParticleSystem punchParticle;

    public void PlayHeadbuttParticle()
    {
        if (headbuttParticle != null)
        {
            headbuttParticle.Play();
        }
    }

    public void PlayPunchParticle()
    {
        if (punchParticle != null)
        {
            punchParticle.Play();
        }
    }
}
