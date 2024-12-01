using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartScene()
    {
        SceneManager.LoadSceneAsync(1);
    }



    public void ExtiGame()
    {
        Application.Quit();
    }
}
