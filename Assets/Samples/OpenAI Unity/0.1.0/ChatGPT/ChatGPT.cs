using ReadyPlayerMe.AvatarLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {

        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject toActivate;
        [SerializeField] private Button button;
        [SerializeField] private GameObject buttonNext;
        [SerializeField] private TextMeshProUGUI textArea;
        public Animator animator; // Tham chi?u ??n Animator component c?a ??i t??ng ng??i ch?i
        public string[] dialogContents;
        public float showingSpeed;
        private int m_senteceindex;

        public Button[] answerButtons;
        private OpenAIApi openai = new OpenAIApi();
        private List<string> answerChoices = new List<string>();
        private int correctIndex { get; set; }
        private string question;
        private int count;
        private string answerplan;
        private int answerrightCount = 0;
        public UnityEvent OnReplyReceived;

        public ColorBlock customColor;
        private bool isSending = false;
        private bool isPlayAgain = false;
        private int numPlays = 0;
        private void Start()
        {
            animator = GetComponent<Animator>();    
            button.onClick.AddListener(PlayAgain);
        }


        private IEnumerator ShowCo()
        {
            textArea.text = "";
            foreach (char letter in dialogContents[m_senteceindex].ToCharArray())
            {
                textArea.text += letter;
                yield return new WaitForSeconds(showingSpeed);
            }

        }
        IEnumerator ExampleCoroutine()
        {
            Debug.Log("Start");
            yield return new WaitForSeconds(15);
            Debug.Log("End");
        }
        private async void SendReply()
        {
            /*     userInput = inputField.text;
                 Instruction += $"{userInput}\nA: ";*/
            try
            {


                StartCoroutine(ShowCo());

                StartCoroutine(ExampleCoroutine());
                var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
                {
                    Model = "text-davinci-002",
                    Prompt = "You are tasked with creating a multiple-choice Java programming question" +
        ". The question should be no more than three lines long and follow the given format:\r\n\r\nQuestion :\r\na.\r\nb.\r\nc.\r\nd.\r\n\r\nAnswer correct: a, b, c, or d\r\n\r\nPlease generate a suitable question, including the options for choices. If the question contains code, ensure it is no more than four lines. Don't forget to include the correct answer at the end, specifying it with \"a\", \"b\", \"c\", or \"d\".\r\n",
                    MaxTokens = 200,
                    Temperature = 0.5f


                });

                OnReplyReceived.Invoke();

                /*            Instruction += $"{completionResponse.Choices[0].Text}\nQ: ";
                */
                string[] choices = { "a", "b", "c", "d" };
                string completionText = completionResponse.Choices[0].Text.Trim();
                var splitIndex = completionText.IndexOf("\n");
                Debug.Log(splitIndex);
                /*            string[] lines = completionText.Split('\n');
                */
                Debug.Log(completionText);
                var index = completionText.IndexOf("A.");
                if (index < 0)
                {
                    index = completionText.IndexOf("a.");
                    if (index < 0)
                    {
                        index = completionText.IndexOf("a)");
                        question = completionText.Substring(0, index);

                    }
                    question = completionText.Substring(0, index);

                }
                else
                {
                    question = completionText.Substring(0, index);
                }

                textArea.text = question;
                count = completionText.IndexOf("Answer correct");
                answerplan = completionText.Substring(count);
                Debug.Log(answerplan);
                // L?y câu h?i (ph?n t? ??u tiên c?a m?ng)
                string textLine = completionText.Substring(index);
                Debug.Log(textLine);
                string[] lines = textLine.Split('\n');
                int correct = 0;
                for (int i = 0; i < 4; i++)
                {
                    string answerCorrect = answerplan.Substring(answerplan.IndexOf(":") + 1).TrimStart();
                    Debug.Log(answerCorrect);
                    string[] answers = new string[4];
                    int count = lines[i].Length;
                    if (count <= 100 && count > 0)
                    {
                        answers[i] = lines[i];

                    }
                    Debug.Log(answers[i]);
                    if(answers[i].Length > 0 || answers[i] != null)
                    {
                        answerButtons[i].GetComponentInChildren<Text>().text = $"{answers[i]}";

                    }
                    else
                    {
                        PlayAgain();
                    }

                    if (answerCorrect.Equals(choices[i]))
                    {
                        correct = i;
                        Debug.Log(correct);
                    }
                    


                }
                correctIndex = correct;
                Debug.Log(correctIndex);

                // Determine which button corresponds to the correct answer

                buttonNext.SetActive(false);
                isSending = false;

            }
            catch (Exception e)
            {
                Debug.LogError(e);

                isSending = false;
            }
        }
        // Complete the instruction

        public void CheckAnswer(int answerIndex)
        {

            string resultText;
            if (answerIndex == correctIndex)
            {
                resultText = "Correct!";
            }
            else
            {
                resultText = "Incorrect!";
            }

            answerButtons[answerIndex].GetComponentInChildren<Text>().text = resultText;
            if (answerButtons[answerIndex].GetComponentInChildren<Text>().text == "Correct!")
            {
                answerrightCount++;
            }

            for (int i = 0; i < answerButtons.Length; i++)
            {

                if (answerButtons[answerIndex].GetComponentInChildren<Text>().text == "Correct!")
                {
                    /*                    animator.SetTrigger("CorrectAnswerTrigger");
                    */
                    textArea.text = $"{answerplan}. Good Job!Keep going";
                    Debug.Log(answerrightCount);
                    answerButtons[answerIndex].GetComponent<Image>().color = new Color(0f, 1f, 0f);

                }
                else
                {
                    answerButtons[answerIndex].GetComponent<Image>().color = new Color(1f, 0f, 0f);
                    textArea.text = $"{answerplan}. Keep going! Never give up";

                }
                answerButtons[i].interactable = false;

            }
            PlayContinue();


        }
        public void PlayAgain()
        {
            string[] choices = { "a", "b", "c", "d" };

            if (numPlays < 5)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    answerButtons[i].interactable = true;
                    answerButtons[i].GetComponent<Image>().color = Color.white;
                    answerButtons[i].GetComponentInChildren<Text>().text = choices[i];
                }
                if (isSending)
                {
                    return;
                }

                isSending = true;
                SendReply();

            }
            else
            {
                isSending = false;
                if (answerrightCount <= 2)
                {
                    textArea.text = $"Never give up! Let's try again {answerrightCount}/5 questions";
                }
                else if (answerrightCount >= 3 && answerrightCount < 5)
                {
                    textArea.text = $"B?n làm r?t t?t r?i ??y! Hãy c? g?ng h?n n?a nhe {answerrightCount}/5 questions";
                }
                else
                {
                    textArea.text = $"You're very good with {answerrightCount}/5 questions";
                }
                Debug.Log("B?n ?ã ch?i quá s? l?n cho phép");

            }
            numPlays++; // T?ng bi?n ??m lên sau m?i l?n ch?i
        }
        IEnumerator PlayNextLienTuc(bool isPlay)
        {
            if (isPlay == true)
            {

                yield return new WaitForSeconds(2);
                PlayAgain();
            }
        }
       
        public void PlayContinue()
        {
            isPlayAgain = true;
            StartCoroutine(PlayNextLienTuc(isPlayAgain));
        }

    }
}