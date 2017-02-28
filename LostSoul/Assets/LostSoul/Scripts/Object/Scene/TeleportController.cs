using UnityEngine;
using System.Collections;

public class TeleportController : MonoBehaviour {

    public bool active = false;
    public Sprite activePort;
    public Sprite inactivePort;
    public Inventory inventory;
    MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
        inventory = FindObjectOfType<Inventory>();
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
            int pos = inventory.itemPositionInInventory(15);
            int medicineToAdd = pos == -1 ? 3 : 3 - inventory.slots[pos].transform.GetChild(0).GetComponent<ItemData>().getAmount();
            for (int i = 0; i < medicineToAdd; i++)
            {
                inventory.AddItem(15);
            }
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
