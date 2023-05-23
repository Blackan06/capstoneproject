using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Transform targetPosition;  // Vị trí đích để di chuyển tới
    private NavMeshAgent navMeshAgent;
    private LineRenderer myLineRender;


    private void Start()
    {
        // Lấy tham chiếu tới NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        myLineRender = GetComponent<LineRenderer>();

        myLineRender.startWidth = 0.15f;
        myLineRender.endWidth = 0.15f;
        myLineRender.positionCount = 0;
    }

    public void ClickToMove(Vector3 vector)
    {
        navMeshAgent.SetDestination(vector);
        NavMeshPath path = new NavMeshPath();
        if (navMeshAgent.CalculatePath(vector, path))
        {
            // Set Line Renderer vertex count
            myLineRender.positionCount = path.corners.Length;

            // Set Line Renderer positions
            for (int i = 0; i < path.corners.Length; i++)
            {
                myLineRender.SetPosition(i, path.corners[i]);
            }
        }
    }

    public void showPath(Vector3 vector)
    {
        if (navMeshAgent.hasPath)
        {
            CalculateAndDrawPath(vector);
        }
        else
        {
            Debug.Log("Path not working");
        }
    }

    private void CalculateAndDrawPath(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        if (navMeshAgent.CalculatePath(destination, path))
        {
            // Set Line Renderer vertex count
            myLineRender.positionCount = path.corners.Length;

            // Set Line Renderer positions
            for (int i = 0; i < path.corners.Length; i++)
            {
                myLineRender.SetPosition(i, path.corners[i]);
            }
        }
    }

}
