using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("AnimationMake1MCFrog");
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }
}