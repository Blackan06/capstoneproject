using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Navigatior : MonoBehaviour
{
    public Button togglePathButton; // Tham chiếu đến nút để tắt chỉ đường
    public Dropdown positionDropdown; // Tham chiếu đến Dropdown chứa các vị trí đến
    public LineRenderer lineRenderer; // Tham chiếu đến Line Renderer để vẽ đường chỉ đường

    private bool isPathVisible = true; // Trạng thái hiển thị của đường chỉ đường
    public NavMeshAgent navMeshAgent; // Tham chiếu đến NavMeshAgent của nhân vật
    public ThirdPersonController thirdPersonController;

    List<Vector3> objectPositions = new List<Vector3>();

    public float distanceThreshold = 1f;
    public LayerMask groundLayer;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        lineRenderer.enabled = true;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;

        objectPositions.Add(new Vector3(-84f, 7f, -14f));
        objectPositions.Add(new Vector3(-56f, 2f, 35f));

        // Xác định số lượng vị trí và tạo danh sách tùy chọn cho Dropdown
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        for (int i = 0; i < objectPositions.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = "Position " + i.ToString();
            options.Add(option);
        }

        // Cập nhật danh sách tùy chọn của Dropdown
        positionDropdown.ClearOptions();
        positionDropdown.AddOptions(options);


        togglePathButton.onClick.AddListener(TogglePathVisibility);
        positionDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(positionDropdown); });
    }

    public void DrawPath(Vector3 targetPosition)
    {
        /*Vector3[] positions = new Vector3[2];
        positions[0] = transform.position; // Vị trí hiện tại của nhân vật
        positions[1] = targetPosition; // Tọa độ đích

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);*/

        // Tạo một đường đi trên NavMesh
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            Vector3[] corners = path.corners;
            lineRenderer.positionCount = corners.Length;

            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 corner = corners[i];

                // Kiểm tra xem điểm có tiếp xúc với mặt đất hay không
                RaycastHit hit;
                if (Physics.Raycast(corner + Vector3.up * 10f, Vector3.down, out hit, 100f, groundLayer))
                {
                    // Điểm tiếp xúc với mặt đất
                    lineRenderer.SetPosition(i, hit.point);
                }
                else
                {
                    // Điểm ẩn
                    lineRenderer.SetPosition(i, corner);
                }
            }
        }

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < distanceThreshold)
        {
            // Tắt LineRenderer
            lineRenderer.enabled = false;
        }
        else
        {
            // Bật LineRenderer
            lineRenderer.enabled = true;
        }
    }


    private void OnDropdownValueChanged(Dropdown dropdown)
    {
        /*        if (isPathVisible)
                {
                    UpdatePath(dropdown.value);
                }*/
        Debug.Log("vi tri: " + dropdown.value + 1);
        int vitri = dropdown.value;
        Vector3 valueSelectedPostion = objectPositions[vitri];
        Debug.Log("gia tri: " + valueSelectedPostion);
        thirdPersonController.setTarget(valueSelectedPostion);
    }

    private void UpdatePath(int targetIndex)
    {
        Vector3 targetPosition = objectPositions[targetIndex];
        navMeshAgent.SetDestination(targetPosition);

        if (isPathVisible)
        {
            ShowLineRenderer(targetPosition);
        }
    }

    private void ShowLineRenderer(Vector3 targetPosition)
    {

        Vector3[] positions = new Vector3[2];
        positions[0] = transform.position; // Vị trí hiện tại của nhân vật
        positions[1] = targetPosition; // Tọa độ đích

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

    }

    private void TogglePathVisibility()
    {
        lineRenderer.enabled = !lineRenderer.enabled;
        Debug.Log(lineRenderer.enabled);
    }

    public void SetDestination(Vector3 destination)
    {
        if (isPathVisible)
        {
            navMeshAgent.SetDestination(destination);
        }
    }
}
