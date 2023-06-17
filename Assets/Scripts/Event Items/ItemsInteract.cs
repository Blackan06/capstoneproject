using UnityEngine;

public class ItemsInteract : MonoBehaviour
{

    [SerializeField] private GameObject checkEventScreen;

    private bool isPlayerInside = false;

    public CharacterController characterController;

    private void Start()
    {
        checkEventScreen.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInside)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("open canvas");

                //hiện canvas event
                checkEventScreen.gameObject.SetActive(true);

                //hiện chuột
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                //vô hiệu hóa nhân vật di chuyển
                characterController.InactiveAgent();
            }
        }
    }

    public void SetPlayerTrue()
    {
        isPlayerInside = true;
    }

    public void SetPlayerFalse()
    {
        isPlayerInside = false;
    }
}
