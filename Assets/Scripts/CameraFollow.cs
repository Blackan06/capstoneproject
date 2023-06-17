using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Transform của nhân vật cần camera theo sau
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Vị trí tương đối của camera so với nhân vật

    private void LateUpdate()
    {
        if (target != null)
        {
            // Cập nhật vị trí của camera dựa trên vị trí của nhân vật và offset
            transform.position = target.position + offset;
        }
    }
}
