using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuInGame : MonoBehaviour {
    public Canvas menu;
    private bool menuOn = false;

    void Start()
    {
        menu.enabled = false;
    }

    void Update()
    {
        if(!menuOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0f;
                menu.enabled = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToGame();
            }
        }
    }

    public void BackToGame()
    {
        Time.timeScale = 1f;
        menu.enabled = false;
        print("fonction menuInGame");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
