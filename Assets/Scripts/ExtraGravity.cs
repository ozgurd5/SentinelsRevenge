using UnityEngine;

public class ExtraGravity : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float extraGravity = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * extraGravity);
    }
}
