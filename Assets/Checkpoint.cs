using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7)
        CheckpointManager.instance.lastCheckpoint = transform.position + Vector3.right*2;
        //AudioSource.PlayClip();
        if(particle != null) particle.Play();
    }
}
