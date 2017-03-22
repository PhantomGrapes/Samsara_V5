using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour {
    CanvasParents[] allCanvas;
    MainCharacter player;
    string currentCanvas;
    public string initialCanvas;
    
	// Use this for initialization
	void Start () {
        allCanvas = FindObjectsOfType<CanvasParents>();
        player = FindObjectOfType<MainCharacter>();
        goToCanvas(initialCanvas);
	}
	
    public void loadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    void escToInit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas(initialCanvas);
        }
    }

	public void goToCanvas(string canvasName)
    {
        for (int i = 0; i < allCanvas.Length; i++)
        {
            if(allCanvas[i].canvasName == canvasName)
            {
                allCanvas[i].GetComponent<Canvas>().enabled = true;
                if (canvasName == "state")
                    Time.timeScale = 1f;
                else
                    Time.timeScale = 0f;
            }
            else
                allCanvas[i].GetComponent<Canvas>().enabled = false;
        }
        currentCanvas = canvasName;
    }

    void caseState()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player.playerOnTeleport)
                goToCanvas("map");
            if (player.playerOnNPC)
                goToCanvas("dialog");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            goToCanvas("inventory");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas("setting");
        }
    }

    void caseSetting()
    {
        escToInit();
    }

    void caseDialog()
    {
        escToInit();
    }

    void caseInventory()
    {
        escToInit();
    }

    void caseMap()
    {
        escToInit();
    }

    void caseMainMenu()
    {

    }

    void caseLoad()
    {
        escToInit();
    }

    void caseThanks()
    {
        escToInit();
    }


    void caseAboutUs()
    {
        escToInit();
    }
    void Update()
    {
        switch (currentCanvas)
        {
            case "setting":
                caseSetting();
                break;
            case "state":
                caseState();
                break;
            case "dialog":
                caseDialog();
                break;
            case "inventory":
                caseInventory();
                break;
            case "map":
                caseMap();
                break;
            case "mainMenu":
                caseMainMenu();
                break;
            case "load":
                caseLoad();
                break;
            case "thanks":
                caseThanks();
                break;
            case "aboutUs":
                caseAboutUs();
                break;
        }
    }
}
