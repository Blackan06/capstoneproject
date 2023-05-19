using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuizManager : MonoBehaviour
{
    private const string apiUrl = "YOUR_API_URL"; // Thay th? b?ng URL API c?a ChatGPT
    public void GenerateQuestion()
    {
        StartCoroutine(CallChatGPTAPI());
    }

    IEnumerator CallChatGPTAPI()
    {
        // G?i c�u h?i t?i API ChatGPT
        string question = "What is the capital of France?";
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "?question=" + question);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Nh?n ph?n h?i JSON t? API
            string jsonResponse = request.downloadHandler.text;

            // X? l� d? li?u JSON ?? l?y c�u h?i v� ?�p �n
            string quizQuestion = ProcessJSONResponse(jsonResponse);

            // T?o c�u h?i tr?c nghi?m trong Unity v?i c�c l?a ch?n v� ?�p �n
            CreateQuizQuestion(quizQuestion);
        }
        else
        {
            Debug.LogError("API request failed with error: " + request.error);
        }
    }

    private string ProcessJSONResponse(string jsonResponse)
    {
        // X? l� v� tr�ch xu?t c�u h?i v� ?�p �n t? JSON
        // ?�y l� n?i b?n s? d?ng c�c th? vi?n JSON ?? ph�n t�ch d? li?u t? ph?n h?i API
        // v� tr�ch xu?t c�u h?i v� ?�p �n t??ng ?ng.
        // V� d?:
        /*
        {
            "question": "What is the capital of France?",
            "options": ["London", "Paris", "Berlin", "Madrid"],
            "answer": "Paris"
        }
        */

        // Tr? v? c�u h?i t? d? li?u JSON
        return "What is the capital of France?";
    }

    private void CreateQuizQuestion(string quizQuestion)
    {
        // T?o c�u h?i tr?c nghi?m trong Unity v?i c�c l?a ch?n v� ?�p �n
        // ?�y l� n?i b?n c� th? s? d?ng c�c ??i t??ng UI trong Unity ?? hi?n th? c�u h?i,
        // c�c l?a ch?n v� ?�p �n cho ng??i ch?i.
    }

}
