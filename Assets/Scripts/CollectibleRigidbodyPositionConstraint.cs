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
        if (!col.collider.CompareTag("Player")) rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}