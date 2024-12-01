using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

public class QuestionPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;

    [SerializeField] private Sprite correctSprite; // Sprite for correct answer
    [SerializeField] private Sprite incorrectSprite; // Sprite for incorrect answer
    [SerializeField] private Sprite defaultSprite; // Default sprite for buttons

    private System.Action<bool> onAnswerSelected; // Action to be invoked when an answer is selected
    private int correctAnswerIndex;

    public void SetQuestion(string question, string[] answers, int correctIndex, System.Action<bool> callback)
    {
        questionText.text = question;
        correctAnswerIndex = correctIndex;
        onAnswerSelected = callback;

        // Reset the sprites to default when a new question is set
        ResetButtonSprites();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];

                int index = i;
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

        // Change the sprite of the clicked button based on correctness
        Button clickedButton = answerButtons[index];
        Image buttonImage = clickedButton.GetComponent<Image>();

        if (isCorrect)
        {
            buttonImage.sprite = correctSprite; // Set correct sprite
        }
        else
        {
            buttonImage.sprite = incorrectSprite; // Set incorrect sprite
        }

        // After showing the feedback, close the panel after a short delay
        StartCoroutine(HandleAnswerFeedback());
    }

    private IEnumerator HandleAnswerFeedback()
    {
        // Wait for 1 second to show the feedback sprite
        yield return new WaitForSeconds(1f);

        // Hide the question panel after feedback
        gameObject.SetActive(false);
    }

    // Resets the sprite of each button to the default sprite
    private void ResetButtonSprites()
    {
        foreach (Button button in answerButtons)
        {
            if (button.gameObject.activeSelf) // Check if button is active
            {
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = defaultSprite; // Set to default sprite
            }
        }
    }
}
