using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
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
        //yield return StartCoroutine(FadeToBlack(1f));
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

        //yield return StartCoroutine(FadeToBlack(0f));
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
        else
        {
         
            LoadMainMenu();

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

    [SerializeField] private VideoClip wrongVideoClip;
    private void PlayWrongVideo()
    {
        if (wrongVideoClip != null)
        {
            isSlowedDown = false;
            videoPlayer.clip = wrongVideoClip;
            videoPlayer.playbackSpeed = 1f;
            videoPlayer.Play();
            Debug.Log($"Playing wrong video: {wrongVideoClip.name}");
        }
        else
        {
            Debug.LogWarning("Wrong video clip is not assigned!");
        }
    }



    private IEnumerator HandleAnswerFeedback(bool isCorrect)
    {
        // Wait for feedback display
        yield return new WaitForSeconds(1f);

        if (isCorrect)
        {
            // If the answer is correct, resume the current video
            ResumeVideo();
        }
        else
        {
            questionPanel.gameObject.SetActive(false);
            yield return StartCoroutine(FadeToBlack(1f));

            // Restart the video from the beginning
            RestartCurrentVideo();

            yield return StartCoroutine(FadeToBlack(0f));
        }
    }

    private void RestartCurrentVideo()
    {
        // Reset the video player to the start of the current clip
        videoPlayer.Stop(); // Stop the current video
        videoPlayer.time = 0f; // Reset the video time to the start
        videoPlayer.playbackSpeed = 1f;
        videoPlayer.Play();   // Start playing the video again from the beginning
        isSlowedDown = false;

        Debug.Log($"Restarting video: {videoPlayer.clip.name}");
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
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
