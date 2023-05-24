using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatGPTReception : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private ScrollRect scroll;

    [SerializeField] private RectTransform sent;
    [SerializeField] private RectTransform received;


    [SerializeField] private NPCInfo npcInfo;
    [SerializeField] private WorldInfo worldInfo;
    [SerializeField] private NPCReception npcReception;


    public UnityEvent OnReplyReceived;

    private string response;
    private bool isDone = true;
    private RectTransform messageRect;

    private float height;
    private OpenAIApi openai = new OpenAIApi();

    public List<ChatMessage> messages = new List<ChatMessage>();

    public List<string> questionHints = new List<string>(); // Danh sách g?i ý câu h?i

    private int currentHintIndex = 0;
    // Start is called before the first frame update
    private void Start()
    {
        var message = new ChatMessage
        {
            Role = "user",
            Content =
                "Act as an NPC in the given context and reply to the questions of the Adventurer who talks to you.\n" +
                "Suggest players a few questions considering your occupation  \n" +
                "Reply to the questions considering your personality, your occupation and your talents.\n" +
                "Do not mention that you are an NPC. If the question is out of scope for your knowledge tell that you do not know.\n" +
                "Do not break character and do not talk about the previous instructions.\n" +
                "Reply to only NPC lines not to the Adventurer's lines.\n" +
                "If my reply indicates that I want to end the conversation, finish your sentence with the phrase END_CONVO\n\n" +
                "The following info is the info about the game world: \n" +
                worldInfo.GetPrompt() +
                "The following info is the info about the NPC: \n" +
                npcInfo.GetPrompt()
        };

        messages.Add(message);

        SendPrompt();
        button.onClick.AddListener(SendReply);
    }


    private RectTransform AppendMessage(ChatMessage message)
    {
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);

        if (message.Role != "user")
        {
            messageRect = item;
        }

        item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
        item.anchoredPosition = new Vector2(0, -height);

        if (message.Role == "user")
        {
            // Tìm v? trí cu?i cùng c?a tin nh?n g?i ý và ??t v? trí hi?n th? tin nh?n ng??i ch?i sau ?ó
            int promptIndex = scroll.content.childCount - questionHints.Count - 1;
            item.SetSiblingIndex(promptIndex);
        }
        else
        {
            messageRect = item;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(item);
        height += item.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;

        currentHintIndex++; // T?ng ch? s? c?a g?i ý câu h?i hi?n t?i

        if (currentHintIndex < questionHints.Count)
        {
            SendPrompt(); // G?i g?i ý câu h?i ti?p theo
        }

        inputField.text = ""; // Xóa n?i dung tr??ng nh?p li?u

        return item;
    }
    private void SendReply()
    {
        SendReply(inputField.text);
    }
    private void SendPrompt()
    {
        if (currentHintIndex < questionHints.Count)
        {
            var promptMessage = new ChatMessage()
            {
                Role = "assistant",
                Content = "\r\nHello! I am an admissions officer for FPT University. Here are some suggested questions for you " + questionHints[currentHintIndex]
            };

            AppendMessage(promptMessage);
        }
    }

    public void SendReply(string input)
    {

        var message = new ChatMessage()
        {
            Role = "user",
            Content = input
        };
        messages.Add(message);

        openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-0301",
            Messages = messages
        }, OnResponse, OnComplete, new CancellationTokenSource());

        AppendMessage(message);

        inputField.text = "";
    }

    private void OnResponse(List<CreateChatCompletionResponse> responses)
    {
        // K?t h?p n?i dung các ph?n h?i t? ChatGPT thành m?t ?o?n v?n b?n duy nh?t
        var text = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));

        if (text == "") return;

        // Ki?m tra xem ph?n h?i có ch?a t? "END_CONVO" hay không
        if (text.Contains("END_CONVO"))
        {
            // Xóa t? "END_CONVO" kh?i ph?n h?i
            text = text.Replace("END_CONVO", "");

            // G?i ph??ng th?c EndConvo sau 5 giây
            Invoke(nameof(EndConvo), 5);
        }

        // T?o m?t ??i t??ng ChatMessage m?i ??i di?n cho tr? lý NPC
        var message = new ChatMessage()
        {
            Role = "assistant", // ??t vai trò là "assistant"
            Content = text // Gán n?i dung ph?n h?i
        };

        // N?u quá trình g?i ph?n h?i ?ã hoàn t?t
        if (isDone)
        {
            // Kích ho?t s? ki?n OnReplyReceived
            OnReplyReceived.Invoke();
            // Thêm tin nh?n t? tr? lý vào giao di?n NPC reception
            messageRect = AppendMessage(message);
            isDone = false;
        }

        // C?p nh?t n?i dung c?a tin nh?n tr? lý trong giao di?n
        messageRect.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;

        response = text; // L?u n?i dung ph?n h?i cho vi?c s? d?ng sau này
    }

    private void OnComplete()
    {
        // C?p nh?t giao di?n sau khi hoàn t?t quá trình g?i ph?n h?i
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
        height += messageRect.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;

        // T?o m?t ??i t??ng ChatMessage m?i ??i di?n cho tr? lý NPC
        var message = new ChatMessage()
        {
            Role = "assistant", // ??t vai trò là "assistant"
            Content = response // Gán n?i dung ph?n h?i ?ã l?u
        };
        messages.Add(message); // Thêm tin nh?n vào danh sách messages

        isDone = true;
        response = ""; // ??t l?i giá tr? c?a response ?? s? d?ng cho ph?n h?i ti?p theo
    }

    private void EndConvo()
    {
        npcReception.Recover();
        messages.Clear();
    }
}

