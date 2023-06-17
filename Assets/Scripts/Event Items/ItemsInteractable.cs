using UnityEngine;

public class ItemsInteractable : MonoBehaviour
{
    [SerializeField] private Canvas checkEventCanvas;

    private void Start()
    {
        //canvas = GameObject.Find("CheckEventScreen").GetComponent<Canvas>();
        GameObject canvasObject = GameObject.Find("CheckEventScreen");
        if (canvasObject != null)
        {
            checkEventCanvas = canvasObject.GetComponent<Canvas>();
        }
    }
    public void Interact()
    {
        if (checkEventCanvas != null)
        {
            checkEventCanvas.gameObject.SetActive(true);
            Debug.Log("Active canvas");
        }
    }
}
