using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    public Vector3 offset = new Vector3(0, 10, 0);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (player == null)
            return;
        transform.position = player.position + offset;
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
