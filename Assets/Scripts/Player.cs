using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction Movement;

    [SerializeField]
    private Camera camera;
    private NavMeshAgent Agent;
    [SerializeField]
    [Range(0, 0.99f)]
    private float Smoothing = 0.25f;
    [SerializeField]
    private float TargetLerpSpeed = 1;

    private Vector3 TargetDirection;
    private float lerpTime = 0;
    private Vector3 LastDirection;
    private Vector3 MovementVector;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        PlayerActionMap = InputActions.FindActionMap("Player");
        Movement = PlayerActionMap.FindAction("Move");
        Movement.started += HandleMovementAction;
        Movement.canceled += HandleMovementAction;
        Movement.performed += HandleMovementAction;
        Movement.Enable();
        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    private void HandleMovementAction(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        MovementVector = new Vector3(input.x, 0, input.y);
    }

    private void Update()
    {
        // Quay nhân vật bằng chuột
        float mouseX = Input.GetAxisRaw("Mouse X");
        transform.Rotate(Vector3.up, mouseX * TargetLerpSpeed * Time.deltaTime);

        MovementVector.Normalize();
        if (MovementVector != LastDirection)
        {
            lerpTime = 0;
        }
        LastDirection = MovementVector;
        TargetDirection = Vector3.Lerp(TargetDirection, MovementVector, Mathf.Clamp01(lerpTime * TargetLerpSpeed * (1 - Smoothing)));

        Agent.Move(TargetDirection * Agent.speed * Time.deltaTime);
        Vector3 lookDirection = MovementVector;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection), Mathf.Clamp01(lerpTime * TargetLerpSpeed * (1 - Smoothing)));
        }

        lerpTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        camera.transform.position = transform.position + Vector3.up * 10;
    }
}
