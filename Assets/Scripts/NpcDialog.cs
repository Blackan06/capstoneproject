using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NpcDialog : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject toActivate;
    [SerializeField] private Transform standingPoint;
    public TextMeshProUGUI dialogText;
    public string[] dialogContents;
    public float showingSpeed;

    private int m_senteceindex;
    private Coroutine m_Coroutine;
    private Transform avatar;
    [SerializeField] private GameObject buttonYes;
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private Button buttonC;
    [SerializeField] private Button buttonD;

    private bool isTextFinished = false;

    private IEnumerator ShowCo()
    {
        dialogText.text = "";
        foreach (char letter in dialogContents[m_senteceindex].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(showingSpeed);  
        }
        isTextFinished = true;

    }

    private IEnumerator WaitForUserInput()
    {
        while (!isTextFinished)
        {
            yield return null;
        }

        // Hiển thị Button Yes
        buttonYes.SetActive(true);
    }
    public void NextDialog()
    {
        if (m_senteceindex < dialogContents.Length)
        {
            m_senteceindex++;
            dialogText.text = "";

            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Coroutine = StartCoroutine(ShowCo());
        }
      
    }
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            avatar = other.transform;

            // disable player input
            avatar.GetComponent<PlayerInput>().DeactivateInput();

            avatar.gameObject.GetComponent<PlayerInput>().enabled = false;

            await Task.Delay(50);
            
            // teleport the avatar to standing point
            avatar.position = standingPoint.position;
            avatar.rotation = standingPoint.rotation;

            // disable main cam, enable dialog cam
            mainCamera.SetActive(false);
            toActivate.SetActive(true);
            StartCoroutine(ShowCo());


            StartCoroutine(WaitForUserInput());

            // dısplay cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void Yes()
    {
        
        if (avatar.GetComponent<PlayerInput>().enabled == true)
        {
            buttonYes.SetActive(false);
            buttonNext.SetActive(false);
        }
        else
        {
            buttonYes.SetActive(false);
            buttonA.gameObject.SetActive(true);
            buttonB.gameObject.SetActive(true);
            buttonC.gameObject.SetActive(true);
            buttonD.gameObject.SetActive(true);
        }
    }

    public void Recover()
    {
        avatar.GetComponent<PlayerInput>().enabled = true;

        mainCamera.SetActive(true);
        toActivate.SetActive(false);
        buttonNext.SetActive(false);
        buttonYes.SetActive(false);
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
        buttonC.gameObject.SetActive(false);
        buttonD.gameObject.SetActive(false);
        buttonA.interactable = true;
        buttonB.interactable = true;
        buttonC.interactable = true;
        buttonD.interactable = true;
        buttonA.GetComponentInChildren<Text>().text = "A";
        buttonB.GetComponentInChildren<Text>().text = "B";
        buttonC.GetComponentInChildren<Text>().text = "C";
        buttonD.GetComponentInChildren<Text>().text = "D";
        buttonA.GetComponent<Image>().color = Color.white;
        buttonB.GetComponent<Image>().color = Color.white;
        buttonC.GetComponent<Image>().color = Color.white;
        buttonD.GetComponent<Image>().color = Color.white;



        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
