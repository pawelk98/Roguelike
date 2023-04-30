using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float moveSpeed = 5.0f;

    Vector3 nextPostion = Vector3.zero;

    void FixedUpdate()
    {
        if (player != null)
        {
            nextPostion = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, nextPostion, Time.deltaTime * moveSpeed);
        }
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
}
