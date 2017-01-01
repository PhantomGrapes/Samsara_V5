using UnityEngine;
using System.Collections;

public class TeleportController : MonoBehaviour {

    public bool active = false;
    public Sprite activePort;
    public Sprite inactivePort;
    private bool playerInCollider = false;
    public MapMenuController map;
	// Use this for initialization
	void Start () {
        map = FindObjectOfType<MapMenuController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (active)
            GetComponent<SpriteRenderer>().sprite = activePort;
        else
            GetComponent<SpriteRenderer>().sprite = inactivePort;
        if (Input.GetKeyDown(KeyCode.F) && playerInCollider)
        {
            map.menuOn = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null) {
            active = true;
            playerInCollider = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null)
        {
            playerInCollider = false; 
        }
    }
}
