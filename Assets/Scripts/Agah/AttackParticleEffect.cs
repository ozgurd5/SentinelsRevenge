using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticleEffect : MonoBehaviour
{
    [SerializeField]ParticleSystem headbuttParticle;
    [SerializeField] ParticleSystem punchParticle;

    void PlayHeadbuttParticle()
    {
        if (headbuttParticle != null)
        {
            headbuttParticle.Play();
        }
    }

    void PlayPunchParticle()
    {
        if (punchParticle != null)
        {
            punchParticle.Play();
        }
    }
}
