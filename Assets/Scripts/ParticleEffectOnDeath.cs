using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectOnDeath : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    public void PlayDeathParticle()
    {
        particle.Play();
    }
}
