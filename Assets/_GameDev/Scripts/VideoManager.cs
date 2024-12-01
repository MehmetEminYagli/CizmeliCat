using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private QuestionAndVideo[] videoInfos;
    [SerializeField] private QuestionPanelController questionPanel;
    [SerializeField] private Image fadeImage;  // The UI Image that will be used for the fade effect

    private int currentVideoIndex = 0;
    private bool isSlowedDown = false;

    private void Start()
    {
        // Subscribe to the event
        videoPlayer.loopPointReached += OnVideoFinished;

        // Initialize the fade image to be fully transparent at the start
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update()
    {
        // Video is playing and it's time to slow down
        if (videoPlayer.isPlaying && !isSlowedDown && videoPlayer.time >= videoInfos[currentVideoIndex].slowdownTime)
        {
            SlowDownVideo(); // Slow down the video
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!questionPanel.gameObject.activeSelf)
        {
            StartCoroutine(FadeOutAndPlayNextVideo());
        }
    }

    private IEnumerator FadeOutAndPlayNextVideo()
    {
        yield return StartCoroutine(FadeToBlack(1f));
        currentVideoIndex++;
        if (currentVideoIndex < videoInfos.Length)
        {
            PlayNextVideo();
        }
        else
        {
            Debug.Log("All videos have been played!");
            EndScene(); // End the scene

            // Game over logic...
        }
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeToBlack(0f));
    }

    private IEnumerator FadeToBlack(float targetAlpha)
    {
        float currentAlpha = fadeImage.color.a;
        float duration = 1f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(currentAlpha, targetAlpha, timeElapsed / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, targetAlpha);
    }

    public void PlayNextVideo()
    {
        if (currentVideoIndex < videoInfos.Length)
        {
            isSlowedDown = false;
            videoPlayer.clip = videoInfos[currentVideoIndex].videoClip;
            videoPlayer.playbackSpeed = 1f;
            videoPlayer.Play();
            Debug.Log($"Playing video: {videoPlayer.clip.name}");
        }
    }

    private void SlowDownVideo()
    {
        isSlowedDown = true;
        videoPlayer.playbackSpeed = videoInfos[currentVideoIndex].slowdownSpeed;
        Debug.Log($"Video slowed down: {videoPlayer.clip.name}");
        Invoke(nameof(ShowQuestion), 2f);
    }

    public void ShowQuestion()
    {
        PauseVideo();
        questionPanel.gameObject.SetActive(true);

        var questionData = videoInfos[currentVideoIndex];
        questionPanel.SetQuestion(questionData.questionText, questionData.answers, questionData.correctAnswerIndex, (isCorrect) =>
        {
            Debug.Log(isCorrect ? "Correct answer!" : "Incorrect answer!");

            // After feedback, close the panel with delay
            StartCoroutine(HandleAnswerFeedback(isCorrect));
        });
    }

    private IEnumerator HandleAnswerFeedback(bool isCorrect)
    {
        // Wait for feedback display
        yield return new WaitForSeconds(1f);  // Adjust the delay as needed

        // If the answer was correct, resume the video
        if (isCorrect)
        {
            ResumeVideo();
        }
        else
        {
            Debug.Log("Wrong answer, try again.");
            // You can add logic to handle wrong answers if needed
        }

        // Hide the question panel after feedback
        questionPanel.gameObject.SetActive(false);
    }

    private void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ResumeVideo()
    {
        videoPlayer.playbackSpeed = 1f;
        videoPlayer.Play();
    }

    public void EndScene()
    {
        Debug.Log("Ending scene...");
        // Scene transition logic can go here
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
