using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    //must be on the same obj.
    JumpSpline jumpSpline;

    private void Start()
    {
        jumpSpline = GetComponent<JumpSpline>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!(other.gameObject.layer == 7)) return; //7 is the Player layer.

        else
        {
            jumpSpline.JumperTransform = other.transform;
            jumpSpline.didJump = false;
        }
    }
}
