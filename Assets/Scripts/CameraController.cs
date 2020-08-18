using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    private Transform player;
    private Vector3 offset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (player)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
            smoothedPosition.z = -2;
            transform.position = smoothedPosition;
        }
    }
}
