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
        // This is called when the video finishes
        if (!questionPanel.gameObject.activeSelf) // Check if the question panel is not active
        {
            // Start the fade-out effect before transitioning to the next video
            StartCoroutine(FadeOutAndPlayNextVideo());
        }
    }

    private IEnumerator FadeOutAndPlayNextVideo()
    {
        // Fade out the current video
        yield return StartCoroutine(FadeToBlack(1f)); // Full fade-out

        // Go to the next video
        currentVideoIndex++;
        if (currentVideoIndex < videoInfos.Length)
        {
            PlayNextVideo(); // Play the next video
        }
        else
        {
            Debug.Log("All videos have been played!");
            EndScene(); // End the scene
        }

        // Wait for a short moment before fading in the next video
        yield return new WaitForSeconds(0.5f);

        // Fade in the next video
        yield return StartCoroutine(FadeToBlack(0f)); // Full fade-in
    }

    private IEnumerator FadeToBlack(float targetAlpha)
    {
        float currentAlpha = fadeImage.color.a;
        float duration = 1f; // Duration of the fade effect
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(currentAlpha, targetAlpha, timeElapsed / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Ensure the final target alpha is set
        fadeImage.color = new Color(0f, 0f, 0f, targetAlpha);
    }

    public void PlayNextVideo()
    {
        if (currentVideoIndex < videoInfos.Length)
        {
            isSlowedDown = false;
            videoPlayer.clip = videoInfos[currentVideoIndex].videoClip;
            videoPlayer.playbackSpeed = 1f; // Reset speed
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
            questionPanel.gameObject.SetActive(false);

            if (isCorrect)
            {
                ResumeVideo();
            }
            else
            {
                Debug.Log("Wrong answer, try again.");
            }
        });
    }

    private void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ResumeVideo()
    {
        videoPlayer.playbackSpeed = 1f; // Reset speed
        videoPlayer.Play();
    }

    public void EndScene()
    {
        Debug.Log("Ending scene...");
        // Scene transition logic can go here
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}



