using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasController : MonoBehaviour {
    CanvasParents[] allCanvas;
    MainCharacter player;
    string currentCanvas;
    
	// Use this for initialization
	void Start () {
        allCanvas = FindObjectsOfType<CanvasParents>();
        player = FindObjectOfType<MainCharacter>();
        goToCanvas("state");
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas("state");
        }
    }

    void caseDialog()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas("state");
        }
    }

    void caseInventory()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas("state");
        }
    }

    void caseMap()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goToCanvas("state");
        }
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
        }
    }
}
