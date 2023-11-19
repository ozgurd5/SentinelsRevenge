using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    public bool move= true;
    public float speed = 15;
    IEnumerator Start()
    {
        Transform target = startPoint;
        while (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) <= 0)
            {
                target = target == startPoint ? endPoint : startPoint;
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 7) return;

        collision.transform.parent = transform;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != 7) return;

        collision.transform.parent = null;
    }


}
