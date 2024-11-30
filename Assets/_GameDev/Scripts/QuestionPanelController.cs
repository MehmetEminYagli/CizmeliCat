using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class QuestionPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;


    private System.Action<bool> onAnswerSelected; // Cevap seçilince çağrılacak işlem
    private int correctAnswerIndex;

    public void SetQuestion(string question, string[] answers, int correctIndex, System.Action<bool> callback)
    {
        questionText.text = question;
        correctAnswerIndex = correctIndex;
        onAnswerSelected = callback;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];

                int index = i; // Lambda için local kopya
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => AnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void AnswerSelected(int index)
    {
        bool isCorrect = index == correctAnswerIndex;
        onAnswerSelected?.Invoke(isCorrect);
    }
}
