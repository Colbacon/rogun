using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private Transform player;
    
    void Start()
    {
        player = Player.instance.transform;
    }

    void LateUpdate()
    {
        if (player)
        {
            //Vector3 desiredPosition = player.position + offset;
            //Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
            //transform.position = smoothedPosition;
            transform.position = player.position + offset;
        }
    }
}
