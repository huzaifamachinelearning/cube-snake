using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 offset;

    void Start()
    {
        // Automatically calculate offset at start
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}