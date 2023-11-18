using UnityEngine;

public class CollectibleRigidbodyPositionConstraint : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision col)
    {
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}