using UnityEngine;
using System.Collections;

public class TeleportController : MonoBehaviour {

    public bool active = false;
    public Sprite activePort;
    public Sprite inactivePort;
    MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
        if (active)
            GetComponent<SpriteRenderer>().sprite = activePort;
        else
            GetComponent<SpriteRenderer>().sprite = inactivePort;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null) {
            active = true;
            player.playerOnTeleport = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null)
        {
            player.playerOnTeleport = false;
        }
    }
}
