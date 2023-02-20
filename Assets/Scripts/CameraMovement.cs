using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
}
