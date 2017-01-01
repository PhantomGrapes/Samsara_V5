using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapMenuController : MonoBehaviour {

    public bool menuOn = false;
    private MainCharacter player;
    public Canvas mapCanvas;
    public TeleportController teleport1;
    public TeleportController teleport2;
    public TeleportController teleport3;
    // Use this for initialization
    void Start () {
        mapCanvas = GetComponent<Canvas>();
        player = FindObjectOfType<MainCharacter>();
	}
	
	// Update is called once per frame
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


    public void goToTeleport1()
    {
        menuOn = false;
        player.transform.position = teleport1.transform.position;
        //print("in fonction1");
    }

    public void goToTeleport2()
    {
        menuOn = false;
        player.transform.position = teleport2.transform.position;
        //print("in fonction2");
    }

    public void goToTeleport3()
    {
        menuOn = false;
        player.transform.position = teleport3.transform.position;
        //print("in fonction3");
    }
}
