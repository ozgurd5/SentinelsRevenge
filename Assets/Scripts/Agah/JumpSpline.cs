using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSpline : MonoBehaviour
{
    [SerializeField] Transform jumpStartPoint; //pointA
    [SerializeField] Transform Handle; //point B
    [SerializeField] Transform jumpEndPoint; // point C

    [HideInInspector]
    public Transform JumperTransform; //The object that gets lerped
    
    [SerializeField][Range(0f,1f)] float lerpSpeedFactor;
    float interpolationValue;

    [HideInInspector]
    public bool didJump = true;
    void Update()
    {
        if (didJump) return;

        interpolationValue = (interpolationValue + Time.deltaTime*lerpSpeedFactor) % 1f;
        //We try to interpolate between pointA and C. We use B to give a curve to the jump.
        JumperTransform.position = QuadraticLerp(jumpStartPoint.position, Handle.position, jumpEndPoint.position, interpolationValue);

        if (Vector3.Distance(JumperTransform.position , jumpEndPoint.position) <0.5f)
        {
            didJump = true;
            StartCoroutine(StopWorking());
            return;
        }
        
    }


    IEnumerator StopWorking()
    {
        enabled = false;
        yield return new WaitForSeconds(2f);
        enabled = true;
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, interpolationValue);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, interpolationValue);
    }


}
