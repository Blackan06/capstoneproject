using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PathDisplay : MonoBehaviour
{
    public Transform target; // Đối tượng đích
    public Text pathText; // Đối tượng Text để hiển thị danh sách khu vực

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        CalculateAndDisplayPath();
    }

    private void Update()
    {
        // Kiểm tra xem có thay đổi vị trí đích không
        if (Vector3.Distance(target.position, agent.destination) > 0.1f)
        {
            CalculateAndDisplayPath();
        }
    }

    private void CalculateAndDisplayPath()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);

        // Cập nhật danh sách khu vực lên đối tượng Text
        pathText.text = "Path: ";
        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 corner = path.corners[i];
            NavMeshHit hit;
            if (NavMesh.SamplePosition(corner, out hit, 0.1f, NavMesh.AllAreas))
            {
                string areaName = NavMesh.GetAreaFromName(hit.mask.ToString()).ToString();
                pathText.text += areaName + " ";
            }
        }
    }
}
