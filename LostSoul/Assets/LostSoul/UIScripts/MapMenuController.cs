using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapMenuController : CanvasParents {

    private MainCharacter player;
    CanvasController canvasController;
    public TeleportController teleport1;
    public TeleportController teleport2;
    public TeleportController teleport3;
    public TeleportController teleport4;
    public TeleportController teleport5;
    public TeleportController teleport6;
    // Use this for initialization
    void Start () {
        canvasName = "map";
        player = FindObjectOfType<MainCharacter>();
        canvasController = FindObjectOfType<CanvasController>();
	}
	
	// Update is called once per frame
    /*
	void Update () {
        mapCanvas.enabled = menuOn;
        if (Input.GetKeyDown(KeyCode.Escape) && menuOn)
            menuOn = false;
        if (menuOn)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
	}
    */


    public void goToTeleport1()
    {
        player.transform.position = teleport1.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction1");
    }

    public void goToTeleport2()
    {
        player.transform.position = teleport2.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction2");
    }

    public void goToTeleport3()
    {
        player.transform.position = teleport3.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction3");
    }
    public void goToTeleport4()
    {
        player.transform.position = teleport4.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction3");
    }
    public void goToTeleport5()
    {
        player.transform.position = teleport5.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction3");
    }
    public void goToTeleport6()
    {
        player.transform.position = teleport6.transform.position;
        canvasController.goToCanvas("state");
        //print("in fonction3");
    }
}
