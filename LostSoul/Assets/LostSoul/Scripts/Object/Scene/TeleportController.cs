using UnityEngine;
using System.Collections;

public class TeleportController : MonoBehaviour {

    public bool active = false;
    public Inventory inventory;
    Animator anim;
    MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
        inventory = FindObjectOfType<Inventory>();
        anim = GetComponent<Animator>();
	}
	

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null) {
            if (!active)
            {
                active = true;
                anim.SetTrigger("ActivePort");
            }
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
