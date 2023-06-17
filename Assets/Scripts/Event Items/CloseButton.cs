using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    public CharacterController characterController;
    private void Start()
    {
        // Lấy tham chiếu đến Canvas
        canvas = GameObject.Find("CheckEventScreen").GetComponent<Canvas>();

        // Lắng nghe sự kiện click của button
        Button closeButton = GetComponent<Button>();
        closeButton.onClick.AddListener(CloseCanvas);
    }

    private void CloseCanvas()
    {
        // Tắt Canvas
        canvas.gameObject.SetActive(false);
        characterController.ActiveAgent();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Canvas is closed");
    }
}
