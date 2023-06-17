using UnityEngine;
using UnityEngine.AI;

public class CharacterControllerMobile : MonoBehaviour
{
    public NavMeshAgent agent;
    public UIVirtualJoystick joystickMove;
    public UIVirtualButton buttonSprint;

    private bool isSprinting = false;

    private void Start()
    {
        // Khởi tạo NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        // Đăng ký các sự kiện từ joystickMove
        joystickMove.joystickOutputEvent.AddListener(MoveCharacter);

        // Đăng ký sự kiện từ buttonSprint
        buttonSprint.buttonStateOutputEvent.AddListener(SetSprint);
    }

    private void MoveCharacter(Vector2 direction)
    {
        // Chỉ di chuyển khi không đang sprint
        if (!isSprinting)
        {
            // Chuyển đổi hướng di chuyển từ joystick thành hướng trong không gian thế giới
            Vector3 worldDirection = new Vector3(direction.x, 0f, direction.y);

            // Đặt vị trí đích cho NavMeshAgent
            agent.SetDestination(transform.position + worldDirection);
        }
    }

    private void SetSprint(bool isSprinting)
    {
        this.isSprinting = isSprinting;
        agent.speed = isSprinting ? agent.speed * 2f : agent.speed;
    }
}
