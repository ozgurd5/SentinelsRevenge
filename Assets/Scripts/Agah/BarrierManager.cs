using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BarrierManager : MonoBehaviour
{
    private Collider[] enemies;
    [SerializeField] GameObject barrier;
    [SerializeField] ParticleSystem barrierPopFX;
    [SerializeField] Vector3 boxBounds;

    private void Start()
    {
        enemies = Physics.OverlapBox(transform.position, boxBounds, quaternion.identity, 8);
    }

    private void Update()
    {
        if(barrier.activeInHierarchy)
            StartCoroutine(CheckEnemies());
    }
    IEnumerator CheckEnemies()
    {
        float aliveEnemies = enemies.Length;
        if (aliveEnemies == 0)
        {
            barrier.SetActive(false);
            barrierPopFX.Play();
            yield break;
        }
        
        foreach (var enemy in enemies)
        {
            Debug.Log(enemy);
            if (true)//enemy==dead
            {
                aliveEnemies--;
            }
        }

        yield return new WaitForSeconds(5 * Time.deltaTime);
    }

}
