using UnityEngine;

public class MakeCanvasLookAtPlayer : MonoBehaviour
{
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        transform.LookAt(playerTransform.position, Vector3.up);

        //Prevent looking down
        if (transform.eulerAngles.x < 270) transform.rotation = Quaternion.Euler(360, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}