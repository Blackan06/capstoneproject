using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private FixedJoystick fixedJoystick;

    private bool isRunning = false;
    private bool isMouseLocked = true;
    private bool isLineVisible = false;
    private bool isAuto = false;
    private bool isAgentEnter = false;
    private bool isWalkBack = false;

    Text textButton;

    public Button togglePathButton;
    public Dropdown positionDropdown;
    public Dropdown showAreaDropdown;
    public LineRenderer lineRenderer;
    public Canvas mainCanvas;
    public Text ShowFootstep;

    float horizontalInput = 0;
    float verticalInput = 0;

    List<Vector3> objectPositions = new List<Vector3>();
    private List<string> areaNames;
    Vector3 movement;

    private Animator animator;
    private NavMeshAgent agent;

    private bool lockCamera = true;

    float xRotation;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        agent.stoppingDistance = 2f;
        agent.speed = moveSpeed;

        textButton = togglePathButton.GetComponentInChildren<Text>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        // Load the JSON file and retrieve the area names
        string filePath = "Assets/Scripts/Json/Area.json";
        string jsonString = File.ReadAllText(filePath);
        areaNames = JsonConvert.DeserializeObject<List<string>>(jsonString);

        mainCanvas.gameObject.SetActive(true);

        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;

        objectPositions = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(-52f, 4f, 25f),
            new Vector3(68f, 0f, 25f),
        };

        for (int i = 0; i < objectPositions.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = "Target " + (i + 1);
            options.Add(option);
        }

        positionDropdown.ClearOptions();
        positionDropdown.AddOptions(options);

        positionDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(positionDropdown); });
    }

    private void Update()
    {
        /* horizontalInput = Input.GetAxis("Horizontal");
         verticalInput = Input.GetAxis("Vertical");*/

        horizontalInput = fixedJoystick.Horizontal;
        verticalInput = fixedJoystick.Vertical;

        Debug.Log("horizontal value:" + horizontalInput);
        Debug.Log("vertical value :" + verticalInput);

        /*if (!isAgentEnter && isMouseLocked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed);
        }*/

        float mountX = 0;
        float mountY = 0;

        if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
        {
            mountX = Touchscreen.current.touches[0].delta.ReadValue().x;
            mountY = Touchscreen.current.touches[0].delta.ReadValue().y;
        }

        xRotation -= mountY * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        transform.Rotate(Vector3.up * mountX * Time.deltaTime);


        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isMouseLocked = !isMouseLocked;
            Debug.Log("mouse lock" + isMouseLocked);

            if (isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("lock mouse");
                agent.isStopped = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("unlock mouse");
                animator.SetBool("isWalk", false);
                animator.SetBool("isRunning", false);
            }
        }


        float currentMoveSpeed = isRunning ? moveSpeed * sprintSpeedMultiplier : moveSpeed;

        Debug.Log("current: " + currentMoveSpeed);

        /*if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f; // Di chuyển theo trục dọc dương (lên)
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f; // Di chuyển theo trục dọc âm (xuống)
        }*/

        if (Input.GetKey(KeyCode.A) || horizontalInput < 0)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalkBack", false);
            animator.SetBool("isTurnLeft", true);
            animator.SetBool("isTurnRight", false);

        }
        else if (Input.GetKey(KeyCode.D) || horizontalInput > 0)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalkBack", false);
            animator.SetBool("isTurnLeft", false);
            animator.SetBool("isTurnRight", true);

        }
        else
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalkBack", false);
            animator.SetBool("isTurnLeft", false);
            animator.SetBool("isTurnRight", false);
        }

        movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        Debug.Log("movement:" + movement);

        transform.Translate(movement * currentMoveSpeed * Time.deltaTime);

        if (movement.magnitude > 0 && verticalInput > 0)
        {
            if (isRunning)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalk", false);
            }
            else
            {
                animator.SetBool("isWalk", true);
                animator.SetBool("isRunning", false);
            }
        }
        else if (movement.magnitude > 0 && verticalInput < 0)
        {
            if (isRunning)
            {
                animator.SetBool("isWalkBack", true);
                animator.SetBool("isRunning", false);
            }
            else
            {
                animator.SetBool("isWalkBack", true);
                animator.SetBool("isWalk", false);
            }
        }
        else if (isAuto)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalkBack", false);
        }

        if (isAuto)
        {
            if (agent.remainingDistance != 0)
            {
                // Kiểm tra nếu agent đã đến điểm đích và dừng di chuyển
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    isAuto = false;
                    agent.ResetPath();
                    lineRenderer.enabled = false;
                    Debug.Log("đã đến");
                }
            }
        }
    }

    public void DrawPath(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            Vector3[] corners = path.corners;
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);

            ActiveAgent();
            lineRenderer.enabled = true;
            isLineVisible = true;
            isAuto = true;

            agent.SetDestination(targetPosition);
            showAreaDropdown.ClearOptions();
            CalculateObjectsOnPath(targetPosition);
        }
    }

    private void OnDropdownValueChanged(Dropdown dropdown)
    {
        int vitri = dropdown.value;
        Vector3 valueSelectedPostion = objectPositions[vitri];
        DrawPath(valueSelectedPostion);

        togglePathButton.gameObject.SetActive(true);
        textButton.text = "Tắt chỉ đường";
        togglePathButton.onClick.AddListener(TogglePathVisibility);
    }

    private void TogglePathVisibility()
    {
        lineRenderer.enabled = false;
        agent.ResetPath();
        isAuto = false;
        togglePathButton.gameObject.SetActive(false);
        showAreaDropdown.gameObject.SetActive(false);
    }

    public void ActiveAgent()
    {
        agent.isStopped = false;
        isAgentEnter = false;
    }

    public void InactiveAgent()
    {
        agent.isStopped = true;
        isAgentEnter = true;
    }
    public void CalculateObjectsOnPath(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        HashSet<string> objectNamesSet = new HashSet<string>();
        showAreaDropdown.gameObject.SetActive(true);
        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            Vector3[] corners = path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Vector3 startPoint = corners[i];
                Vector3 endPoint = corners[i + 1];

                // Tìm các đối tượng nằm trên đoạn đường từ startPoint đến endPoint
                Collider[] colliders = Physics.OverlapCapsule(startPoint, endPoint, agent.radius);

                foreach (Collider collider in colliders)
                {
                    // Xử lý đối tượng (ví dụ: lưu lại, hiển thị, ...)
                    GameObject obj = collider.gameObject;
                    string objName = obj.name;
                    if (!objectNamesSet.Contains(objName))
                    {
                        objectNamesSet.Add(objName);
                    }
                }
            }
        }
        int count = objectNamesSet.Count;
        List<string> element = new List<string>(objectNamesSet);
        List<string> options = new List<String>();
        options.Insert(0, "Đây là những chỗ bạn cần đi");
        String textToShow = "";
        foreach (String ele in element)
        {
            if (areaNames.Contains(ele))
            {
                Debug.Log(ele);
                options.Add(ele);
                textToShow += ele + "\n";
            }
        }
        //ShowFootstep.text = textToShow;

        showAreaDropdown.AddOptions(options);
        showAreaDropdown.onValueChanged.RemoveAllListeners();
    }

    public void SetMovingFromJoytick(float valueHorizontal, float valueVertical)
    {
        horizontalInput = valueHorizontal;
        verticalInput = valueVertical;
    }
}
