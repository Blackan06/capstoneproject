/*using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MobileCharacterController : MonoBehaviour
{
    public Joystick joystick;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (movement.magnitude > 0)
        {
            // Chuẩn hóa vector di chuyển để có độ dài là 1
            movement.Normalize();

            // Di chuyển nhân vật đến vị trí mới trên NavMesh
            agent.SetDestination(transform.position + movement);
        }
    }
}
*/