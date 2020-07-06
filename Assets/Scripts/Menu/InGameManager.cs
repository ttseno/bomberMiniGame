using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject inGameMenuUI;
    public TextMeshProUGUI menuText;
    public Button resumeButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        inGameMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    
    public void Resume()
    {
        inGameMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void EndGame()
    {
        print("GAME OVER");
        menuText.text = "GAME OVER";
        inGameMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        resumeButton.gameObject.SetActive(false);
    }

    public void Restart()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
      
    public void GoToMainMenu()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
}
