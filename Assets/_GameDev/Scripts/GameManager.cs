using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [SerializeField] private VideoManager videoManager;
    [SerializeField] private VideoClip initialClip;

    private void Start()
    {
        // Initially play the first video if needed
        videoManager.PlayNextVideo();
    }

    public void NextInteractiveScene()
    {
        // Load the next interactive scene when called
        Debug.Log("Loading next interactive movie...");
        // Here, you can add logic for loading the next scene, such as:
        // SceneManager.LoadScene("NextScene");
    }
}
