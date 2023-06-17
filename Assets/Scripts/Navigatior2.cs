using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Navigatior2 : MonoBehaviour
{
    public Button togglePathButton; // Tham chiếu đến nút để tắt chỉ đường
    public Dropdown positionDropdown; // Tham chiếu đến Dropdown chứa các vị trí đến
    public LineRenderer lineRenderer; // Tham chiếu đến Line Renderer để vẽ đường chỉ đường

    private bool isPathVisible = true; // Trạng thái hiển thị của đường chỉ đường
    public NavMeshAgent navMeshAgent; // Tham chiếu đến NavMeshAgent của nhân vật

    List<Vector3> objectPositions = new List<Vector3>();

    public float distanceThreshold = 1f;
    public LayerMask groundLayer;

    private bool isLineVisible = false;
    private void Start()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();


        navMeshAgent = GetComponent<NavMeshAgent>();

        lineRenderer.enabled = true;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;


        objectPositions = new List<Vector3>()
    {
        new Vector3(9f, 10f, 30f),
        new Vector3(-56f, 2f, 35f),
    };

        for (int i = 0; i < objectPositions.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();

            option.text = "phòng " + (i + 1); ;

            options.Add(option);
        }

        // Cập nhật danh sách tùy chọn của Dropdown
        positionDropdown.ClearOptions();
        positionDropdown.AddOptions(options);


        positionDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(positionDropdown); });
    }


    public void DrawPath(Vector3 targetPosition)
    {
        //chỉ show line dưới chân, không di chuyển
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            Vector3[] corners = path.corners;

            // Cập nhật số lượng điểm trong LineRenderer
            lineRenderer.positionCount = corners.Length;

            // Cập nhật tọa độ các điểm trong LineRenderer
            lineRenderer.SetPositions(corners);

            isLineVisible = true;

            navMeshAgent.SetDestination(targetPosition);
        }

        if (isLineVisible && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // Xóa LineRenderer
            lineRenderer.positionCount = 0;

            isLineVisible = false;
        }
    }


    private void OnDropdownValueChanged(Dropdown dropdown)
    {

        Debug.Log("vi tri: " + dropdown.value + 1);
        int vitri = dropdown.value;
        Vector3 valueSelectedPostion = objectPositions[vitri];
        Debug.Log("gia tri: " + valueSelectedPostion);
        DrawPath(valueSelectedPostion);

        Text textButton = togglePathButton.gameObject.GetComponent<Text>();

        togglePathButton.gameObject.SetActive(true);
        textButton.text = "ẩn chỉ đường";
        togglePathButton.onClick.AddListener(TogglePathVisibility);
    }


    private void TogglePathVisibility()
    {
        lineRenderer.enabled = !lineRenderer.enabled;
        Debug.Log(lineRenderer.enabled);
    }
}
