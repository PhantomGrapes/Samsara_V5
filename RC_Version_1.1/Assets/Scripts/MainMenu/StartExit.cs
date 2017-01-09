using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartExit : MonoBehaviour {
    
    public void StartGame()
    {
        SceneManager.LoadScene("BuildRobot");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
