using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{

    [SerializeField] private CharacterController characterController;

    [SerializeField] private Canvas checkEventCanvas;

    [SerializeField] private ItemsInteract itemsInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter");
            itemsInteract.SetPlayerTrue();
        }
    }

}

